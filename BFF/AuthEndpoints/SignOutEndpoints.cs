using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using static BFF.Utils;

namespace BFF.AuthEndpoints
{
    public static class SignOutEndpoints
    {
       public static void MapSignOutEndpoints(this WebApplication app) {
            app.MapGet("/logout",[Authorize] async (HttpContext http,IConfiguration configuration) =>
            {
                //RedirectUri= "/spa/landing_page"
                // In case of options.SaveTokens = true;

                //http.Session.Clear();
                //await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                //await http.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                //{
                //    RedirectUri= "http://localhost:5173",
                //});

                // In case of options.SaveTokens = false;
                //var idToken = http.Items["id_token"] as string;

                //var logoutUrl = $"{configuration["OIDC:Authority"]}/protocol/openid-connect/logout" +
                //$"?id_token_hint={idToken}" +
                //$"&post_logout_redirect_uri=https://localhost:7117/spa/landing_page";

                //http.Session.Clear();
                //await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                //return Results.Redirect(logoutUrl);
                var idToken = http.Session.GetString("id_token");
                return await SignOut(http, configuration, idToken);
            }).WithName("SignOutEndpoint").WithDisplayName("SignOutEndpoint");
        }
    }
}
