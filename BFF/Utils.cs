using System.Text.Json;
using BFF.AuthEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using static System.Net.WebRequestMethods;
using static BFF.Models;

namespace BFF
{
    public static class Utils
    {
        public static async Task<IResult> SignOut(HttpContext http, IConfiguration configuration,string? idToken) {
            var logoutUrl = $"{configuration["OIDC:Authority"]}/protocol/openid-connect/logout" +
            $"?id_token_hint={idToken}" +
            $"&post_logout_redirect_uri=https://localhost:7117/spa/landing_page";

            http.Session.Clear();
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Redirect(logoutUrl);
        }

        //public static async Task RefreshToken(HttpContext http, IConfiguration configuration)
        //{
        //    var refreshToken = http.Session.GetString("refresh_token");
        //    var client = new HttpClient();
        //    var response = await client.PostAsync($"https://127.0.0.1:8080/realms/myrealm/protocol/openid-connect/token",
        //        new FormUrlEncodedContent(new Dictionary<string, string>
        //        {
        //            ["grant_type"] = "refresh_token",
        //            ["client_id"] = configuration["OIDC:ClientId"]!,
        //            ["client_secret"] = configuration["OIDC:ClientSecret"]!,
        //            ["refresh_token"] = refreshToken!
        //        }));
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return;
        //        //app.MapSignOutEndpoints();
        //        var idToken = http.Session.GetString("id_token");

        //        var logoutUrl = $"{configuration["OIDC:Authority"]}/protocol/openid-connect/logout" +
        //        $"?id_token_hint={idToken}" +
        //        $"&post_logout_redirect_uri=https://localhost:7117/spa/landing_page";

        //        http.Session.Clear();
        //        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        Results.Redirect(logoutUrl);
        //        //throw new Exception("Refresh failed");

        //    }

        //    //var json = await response.Content.ReadAsStringAsync();
        //    //var token = JsonSerializer.Deserialize<TokenResponse>(json);
        //    // OR
        //    var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        //    // Update session
        //    http.Session.SetString("access_token", token!.AccessToken!);
        //    http.Session.SetString("refresh_token", token.RefreshToken!);
        //    http.Session.SetString("id_token", token.IdToken!);

        //    http.Session.SetString("expires_at",
        //        DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn)
        //        .ToString("O"));

        //}

        //public static async Task<string> GetValidAccessToken(HttpContext http, IConfiguration configuration)
        //{
        //    var expiresAtStr = http.Session.GetString("expires_at");
        //    var expirationTime = DateTime.Parse(expiresAtStr!);
        //    if (expirationTime <= DateTime.UtcNow || expirationTime <= DateTime.UtcNow.AddSeconds(30))
        //    {
        //        await RefreshToken(http,configuration);
        //    }

        //    return http.Session.GetString("access_token")!;
        //}
    }
}
