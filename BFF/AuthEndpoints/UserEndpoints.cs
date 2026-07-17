using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BFF.AuthEndpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapGet("/userinfo", [Authorize] (HttpContext http) =>
            {
                //var username = http.User.Identity?.Name;
                var username = http.User.FindFirst(ClaimTypes.Name)?.Value;
                return Results.Ok(new { Username = username });
            })
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
