using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using static BFF.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BFF
{
    public class Models
    {
        public record TokenData (
            string AccessToken,
            string RefreshToken,
            string IdToken,
            DateTimeOffset ExpiresAt,
            DateTimeOffset LastIntrospectedAt
        );

        public interface ITokenStore
        {
            Task SaveAsync(string sessionId, TokenData tokens, CancellationToken ct = default);
            Task<TokenData?> GetAsync(string sessionId, CancellationToken ct = default);
            Task RemoveAsync(string sessionId, CancellationToken ct = default);
        }

        public class DistributedTokenStore(IDistributedCache cache) : ITokenStore
        {
            private readonly IDistributedCache _cache = cache;
            private static string Key(string sessionId) => $"tokens:{sessionId}";

            public async Task SaveAsync(string sessionId, TokenData tokens, CancellationToken ct = default) {
                var json = JsonSerializer.Serialize(tokens);
                await _cache.SetStringAsync(Key(sessionId), json, new DistributedCacheEntryOptions
                {
                    // Keep entry alive slightly beyond refresh token lifetime
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    //AbsoluteExpirationRelativeToNow = tokens.ExpiresAt - DateTimeOffset.UtcNow
                }, ct);
            }
            public async Task<TokenData?> GetAsync(string sessionId, CancellationToken ct = default) {
                var json = await _cache.GetStringAsync(Key(sessionId), ct);
                return json is null ? null : JsonSerializer.Deserialize<TokenData>(json);
            }
            public async Task RemoveAsync(string sessionId, CancellationToken ct = default) 
                => await _cache.RemoveAsync(Key(sessionId), ct);
        }

        public interface ITokenRefreshService
        {
            /// <summary>
            /// Returns a valid access token, refreshing if expired.
            /// Returns null if refresh fails (session must be terminated).
            /// </summary>
            /// 

            //Task<string?> GetValidAccessTokenAsync() in case we return just access token
            Task<string?> GetValidAccessTokenAsync(HttpContext httpContext, CancellationToken ct = default);
        }

        public class KeycloakTokenRefreshService(ITokenStore tokenStore, IHttpClientFactory httpClientFactory,IConfiguration configuration, ILogger<KeycloakTokenRefreshService> logger) : ITokenRefreshService
        {
            private readonly ITokenStore _tokenStore = tokenStore;
            private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
            private readonly IConfiguration _config = configuration;
            private readonly ILogger<KeycloakTokenRefreshService> _logger = logger;
            private readonly TimeSpan _introspectInterval = TimeSpan.FromMinutes(
            configuration.GetValue<double>("Bff:IntrospectIntervalMinutes", 5));

            // Clock skew buffer — refresh 30s before actual expiry
            private static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(30);

            //SemaphoreSlim is a lightweight synchronization primitive in C# used to limit
            //the number of threads or tasks that can access a specific resource or block of code concurrently.
            //It is the recommended, highly efficient solution for throttling
            //and in-process concurrency management within a single application.
            private static readonly ConcurrentDictionary<string, SemaphoreSlim> _sessionLocks = new();

            //public async Task<string?> GetValidAccessTokenAsync() in case we return just access token
            public async Task<string?> GetValidAccessTokenAsync(HttpContext httpContext, CancellationToken ct = default)
            {
                var sessionId = httpContext.Session.Id; // Assuming session ID is used as the key for token storage
                var userId = httpContext.User?.FindFirst("sub")?.Value; // used as key instead of sessionId if you want to store tokens per user instead of per session
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No 'sub' claim on principal — cannot look up tokens");
                    return null;
                }

                var tokensData = await _tokenStore.GetAsync(userId, ct);

                if (tokensData is null)
                {
                    _logger.LogWarning("No tokens found for user {UserId}", userId);
                    return null;
                }

                var needsRefresh = DateTimeOffset.UtcNow >= tokensData.ExpiresAt - ClockSkew;
                var needsIntrospection = !needsRefresh && DateTimeOffset.UtcNow >= tokensData.LastIntrospectedAt + _introspectInterval;

                // Fast path — token valid, introspect not due yet
                if (!needsRefresh && !needsIntrospection)
                {
                    return tokensData.AccessToken;
                }
                return await WithSessionLockAsync(userId, tokensData, needsRefresh, ct);
            }

            //private async Task<string?> RefreshAsync() in case we return just access token
            private async Task<string?> RefreshAsync(string userId, string refreshToken, CancellationToken ct)
            {
                //var client = _httpClientFactory.CreateClient("KeycloakClient");
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(@$"{_config["OIDC:Authority"]}/protocol/openid-connect/token", 
                    new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            ["grant_type"] = "refresh_token",
                            ["client_id"] = _config["OIDC:ClientId"]!,
                            ["client_secret"] = _config["OIDC:ClientSecret"]!,
                            ["refresh_token"] = refreshToken
                        }),
                    ct);

                if (!response.IsSuccessStatusCode) {
                    _logger.LogWarning("Failed to refresh token for user {UserId}", userId);
                    await _tokenStore.RemoveAsync(userId, ct);
                    _sessionLocks.TryRemove(userId, out _); // session is dead, drop the lock
                    return null; // Caller should sign out
                }

                var payload = await response.Content.ReadFromJsonAsync<TokenRefreshResponse>(cancellationToken: ct);
                var newTokenData = new TokenData(
                    AccessToken: payload!.AccessToken,
                    RefreshToken: payload.RefreshToken ?? refreshToken, // Keycloak may rotate it
                    IdToken: payload.IdToken,
                    ExpiresAt: DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn),
                    LastIntrospectedAt: DateTimeOffset.UtcNow
                );

                await _tokenStore.SaveAsync(userId, newTokenData, ct);
                return newTokenData.AccessToken;
            }

            private async Task<string?> IntrospectAsync(string userId, TokenData tokenData, CancellationToken ct) {
                //var client = _httpClientFactory.CreateClient("KeycloakClient");
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(@$"{_config["OIDC:Authority"]}/protocol/openid-connect/token/introspect",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["client_id"] = _config["OIDC:ClientId"]!,
                        ["client_secret"] = _config["OIDC:ClientSecret"]!,
                        ["token"] = tokenData.AccessToken,
                        ["token_type_hint"] = "access_token"
                    }),
                    ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Introspect HTTP error for user {UserId}: {Status}",
                        userId, response.StatusCode);
                    return tokenData.AccessToken; // Keycloak unreachable — fail open, keep token
                }

                var result = await response.Content
                    .ReadFromJsonAsync<IntrospectionResponse>(cancellationToken: ct);

                if (result?.Active != true)
                {
                    // Session was revoked server-side (admin logout, forced session end, etc.)
                    _logger.LogWarning(
                        "Token is no longer active for user {UserId} — session revoked",
                        userId);

                    await _tokenStore.RemoveAsync(userId, ct);
                    _sessionLocks.TryRemove(userId, out _); // session is dead, drop the lock
                    return null; // triggers 401 + SignOut in middleware

                }
                // Update introspection timestamp
                var updatedTokenData = tokenData with { LastIntrospectedAt = DateTimeOffset.UtcNow };
                await _tokenStore.SaveAsync(userId, updatedTokenData, ct);

                _logger.LogDebug(
                    "Token confirmed active for user {UserId}, next check at {NextCheck}",
                    userId, updatedTokenData.LastIntrospectedAt + _introspectInterval);
                return updatedTokenData.AccessToken;
            }

            private async Task<string?> WithSessionLockAsync(string userId, TokenData tokenData, bool needsRefresh, CancellationToken ct)
            {
                var semaphore = _sessionLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync(ct);
                try
                {
                    // Re-fetch in case it was updated while waiting
                    tokenData = await _tokenStore.GetAsync(userId, ct) ?? tokenData;
                    needsRefresh = DateTimeOffset.UtcNow >= tokenData.ExpiresAt - ClockSkew;
                    var needsIntrospection = !needsRefresh 
                        && DateTimeOffset.UtcNow >= tokenData.LastIntrospectedAt+_introspectInterval;
                    if (!needsRefresh && !needsIntrospection) {
                        return tokenData.AccessToken; // Another request already refreshed it
                    }else if (needsRefresh)
                    {
                        return await RefreshAsync(userId, tokenData.RefreshToken, ct);
                    } else if (needsIntrospection)
                    {
                        return await IntrospectAsync(userId, tokenData, ct);
                        //return accessToken;
                    }

                    // Token was refreshed by a concurrent request while this one waited on the semaphore
                    return tokenData.AccessToken;
                }
                finally
                {
                    semaphore.Release();
                    //if(semaphore.CurrentCount == 1) // No one is waiting, clean up
                    //{
                        //_sessionLocks.TryRemove(sessionId, out _);
                        //The check CurrentCount == 1 means "no one is waiting right now",
                        //but there's a window between Release() and the if check where
                        //another thread can call GetOrAdd and grab a reference to the same semaphore object.
                        //Then you remove it from the dictionary — but that other thread
                        //is still holding a reference and will WaitAsync on a semaphore that's now orphaned
                        //and will never be cleaned up. Worse, a third thread will then create a new semaphore
                        //for the same session, breaking the mutual exclusion guarantee entirely.

                        //Using the KeyValuePair overload of TryRemove is atomic — it only removes the entry if
                        //the key still maps to this exact semaphore instance, preventing the orphan scenario.
                        //_sessionLocks.TryRemove(new KeyValuePair<string, SemaphoreSlim>(sessionId, semaphore));
                    //}
                }
            }
        }

        


        // Maps Keycloak's /token/introspect endpoint JSON response :
        // Active property indicates if the token is still valid (not expired or revoked)
        public class IntrospectionResponse
        {
            [JsonPropertyName("active")] public bool Active { get; set; }
        }

        // Maps Keycloak's /token endpoint JSON response
        public class TokenRefreshResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = "";

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("id_token")]
            public string IdToken { get; set; } = "";

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
