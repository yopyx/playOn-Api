using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using static BFF.Models;

namespace BFF
{
    public class Models
    {
        public record TokenData (
            string AccessToken,
            string RefreshToken,
            string IdToken,
            DateTimeOffset ExpiresAt
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
            Task<TokenData?> GetValidAccessTokenAsync(HttpContext httpContext, CancellationToken ct = default);
        }

        public class KeycloakTokenRefreshService(ITokenStore tokenStore, IHttpClientFactory httpClientFactory,IConfiguration configuration, ILogger<KeycloakTokenRefreshService> logger) : ITokenRefreshService
        {
            private readonly ITokenStore _tokenStore = tokenStore;
            private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
            private readonly IConfiguration _config = configuration;
            private readonly ILogger<KeycloakTokenRefreshService> _logger = logger;

            // Clock skew buffer — refresh 30s before actual expiry
            private static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(30);

            //public async Task<string?> GetValidAccessTokenAsync() in case we return just access token
            public async Task<TokenData?> GetValidAccessTokenAsync(HttpContext httpContext, CancellationToken ct = default) {
                var sessionId = httpContext.Session.Id;
                var tokensData = await _tokenStore.GetAsync(sessionId, ct);

                if(tokensData is null)
                {
                    _logger.LogWarning("No tokens found for session {SessionId}", sessionId);
                    return null;
                }

                // Token still valid
                if(tokensData.ExpiresAt - ClockSkew > DateTimeOffset.UtcNow)
                {
                    return tokensData;
                }

                // Token expired — attempt refresh
                _logger.LogInformation("Access token expired for session {SessionId}, refreshing...", sessionId);
                return await RefreshAsync(sessionId, tokensData.RefreshToken, ct);
            }

            //private async Task<string?> RefreshAsync() in case we return just access token
            private async Task<TokenData?> RefreshAsync(string sessionId, string refreshToken, CancellationToken ct)
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync($"{_config["OIDC:Authority"]}/protocol/openid-connect/token", 
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
                    ExpiresAt: DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn)
                );

                await _tokenStore.SaveAsync(sessionId, newTokenData, ct);
                return newTokenData;
            }
        }

        // Maps Keycloak's JSON response
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
