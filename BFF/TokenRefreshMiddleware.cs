using BFF.AuthEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;
using static BFF.Models;
using static BFF.Utils;

namespace BFF
{
    public class TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<TokenRefreshMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, ITokenRefreshService refreshService,IConfiguration configuration)
        {
            // Only apply to authenticated API routes
            if (context.User.Identity?.IsAuthenticated == true &&
                context.Request.Path.StartsWithSegments("/resources_api"))
            {
                await context.Session.LoadAsync();
                _logger.LogWarning("Session ID: {SessionId}, IsAvailable: {IsAvailable}",
                    context.Session.Id,
                    context.Session.IsAvailable);

                var idToken = context.Session.GetString("id_token");
                _logger.LogWarning("id_token present: {Present}", idToken is not null);

                var accessToken = await refreshService.GetValidAccessTokenAsync(context);

                if (accessToken is null)
                {
                    // Only sign out if the token store confirms the session is gone
                    //var sessionStillExists = await refreshService.(sessionId);
                    //if (!sessionStillExists)
                    //{
                    //    await SignOut(context, configuration, idToken);
                    //    return;
                    //}

                    // Transient failure — return 401, don't nuke the session
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;

                    //await SignOut(context, configuration, idToken);
                    //return;
                }
                else
                {
                    // Attach token to the request context so downstream handlers can use it
                    context.Items["access_token"] = accessToken;
                }
            }

            await _next(context);
        }
    }
}
