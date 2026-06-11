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
                return Results.Challenge(new AuthenticationProperties { RedirectUri = "https://localhost:7117/spa/main_page" },
                              [OpenIdConnectDefaults.AuthenticationScheme]);
            });
        }
    }
}
