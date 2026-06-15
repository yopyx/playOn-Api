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
                var sessionId = httpContext.Session.Id;
                var tokensData = await _tokenStore.GetAsync(sessionId, ct);

                if (tokensData is null)
                {
                    _logger.LogWarning("No tokens found for session {SessionId}", sessionId);
                    return null;
                }

                var needsRefresh = DateTimeOffset.UtcNow >= tokensData.ExpiresAt - ClockSkew;
                var needsIntrospection = !needsRefresh && DateTimeOffset.UtcNow >= tokensData.LastIntrospectedAt + _introspectInterval;

                // Fast path — token valid, introspect not due yet
                if (!needsRefresh && !needsIntrospection)
                {
                    return tokensData.AccessToken;
                }
                return await WithSessionLockAsync(sessionId, tokensData, needsRefresh, ct);
            }

            //private async Task<string?> RefreshAsync() in case we return just access token
            private async Task<string?> RefreshAsync(string sessionId, string refreshToken, CancellationToken ct)
            {
                var client = _httpClientFactory.CreateClient("KeycloakClient");
                var response = await client.PostAsync("/token", 
                    new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            ["grant_type"] = "refresh_token",
                            ["client_id"] = _config["OIDC:ClientId"]!,
                            ["client_secret"] = _config["OIDC:ClientSecret"]!,
                            ["refresh_token"] = refreshToken
                        }),
                    ct);

                if (!response.IsSuccessStatusCode) {
                    _logger.LogWarning("Failed to refresh token for session {SessionId}", sessionId);
                    await _tokenStore.RemoveAsync(sessionId, ct);
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

                await _tokenStore.SaveAsync(sessionId, newTokenData, ct);
                return newTokenData.AccessToken;
            }

            private async Task<string?> IntrospectAsync(string sessionId, TokenData tokenData, CancellationToken ct) {
                var client = _httpClientFactory.CreateClient("KeycloakClient");
                var response = await client.PostAsync("/token/introspect",
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
                        "Introspect HTTP error for session {SessionId}: {Status}",
                        sessionId, response.StatusCode);
                    return tokenData.AccessToken; // Keycloak unreachable — fail open, keep token
                }

                var result = await response.Content
                    .ReadFromJsonAsync<IntrospectionResponse>(cancellationToken: ct);

                if (result?.Active != true)
                {
                    // Session was revoked server-side (admin logout, forced session end, etc.)
                    _logger.LogWarning(
                        "Token is no longer active for session {SessionId} — session revoked",
                        sessionId);

                    await _tokenStore.RemoveAsync(sessionId, ct);
                    return null; // triggers 401 + SignOut in middleware

                }
                // Update introspection timestamp
                var updatedTokenData = tokenData with { LastIntrospectedAt = DateTimeOffset.UtcNow };
                await _tokenStore.SaveAsync(sessionId, updatedTokenData, ct);

                _logger.LogDebug(
                    "Token confirmed active for session {SessionId}, next check at {NextCheck}",
                    sessionId, updatedTokenData.LastIntrospectedAt + _introspectInterval);

                return updatedTokenData.AccessToken;
            }

            private async Task<string?> WithSessionLockAsync(string sessionId, TokenData tokenData, bool needsRefresh, CancellationToken ct)
            {
                var semaphore = _sessionLocks.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync(ct);
                try
                {
                    // Re-fetch in case it was updated while waiting
                    tokenData = await _tokenStore.GetAsync(sessionId, ct) ?? tokenData;
                    needsRefresh = DateTimeOffset.UtcNow >= tokenData.ExpiresAt - ClockSkew;
                    var needsIntrospection = !needsRefresh 
                        && DateTimeOffset.UtcNow >= tokenData.LastIntrospectedAt+_introspectInterval;
                    if (!needsRefresh && !needsIntrospection) {
                        return tokenData.AccessToken; // Another request already refreshed it
                    }else if (needsRefresh)
                    {
                        return await RefreshAsync(sessionId, tokenData.RefreshToken, ct);
                    } else if (needsIntrospection)
                    {
                        return await IntrospectAsync(sessionId, tokenData, ct);
                        //return accessToken;
                    }
                    return tokenData.AccessToken; // Shouldn't reach here
                }
                finally
                {
                    semaphore.Release();
                    if(semaphore.CurrentCount == 1) // No one is waiting, clean up
                    {
                        _sessionLocks.TryRemove(sessionId, out _);
                    }
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
