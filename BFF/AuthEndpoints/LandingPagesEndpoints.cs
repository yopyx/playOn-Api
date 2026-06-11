using Microsoft.AspNetCore.Authorization;

namespace BFF.AuthEndpoints
{
    public static class LandingPagesEndpoints
    {
        public static void MapLandingPagesEndpoints(this WebApplication app) {
            
            // Redirecting to the main page after successful login.
            app.MapGet("/spa/main_page",[Authorize] (IConfiguration configuration) => {
                return Results.Redirect(configuration["SPA:LandingPagePostLogin"]!);
            });

            // Redirecting to the landing page after successful logout.
            app.MapGet("/spa/landing_page", (IConfiguration configuration) => {
                return Results.Redirect(configuration["SPA:LandingPagePostLogout"]!);
            });
        }
    }
}
