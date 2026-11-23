using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BFF.AuthEndpoints
{
    public static class SignInEndpoints
    {
        public static void MapSignInEndpoints(this WebApplication app)
        {
            app.MapGet("/login", () =>
            {
                // RedirectUri = $"{app.Configuration["Bff:BaseUrl"]}/spa/main_page"
                return Results.Challenge(new AuthenticationProperties { RedirectUri = app.Configuration["SPA:LandingPagePostLogin"] },
                              [OpenIdConnectDefaults.AuthenticationScheme]);
            });
        }
    }
}
