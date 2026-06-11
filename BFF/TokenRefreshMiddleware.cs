using BFF.AuthEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;
using static BFF.Models;
using static BFF.Utils;

namespace BFF
{
    public class TokenRefreshMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, ITokenRefreshService refreshService)
        {
            // Only apply to authenticated API routes
            if (context.User.Identity?.IsAuthenticated == true &&
                context.Request.Path.StartsWithSegments("/resources_api")
                || context.Request.Path.StartsWithSegments("/checkIfLogged"))
            {
                await context.Session.LoadAsync();
                var tokenData = await refreshService.GetValidAccessTokenAsync(context);

                if (tokenData is null)
                {
                    //await SignOut(context, configuration);
                    //context.Request.Path = "/logout";

                    var endpoint = context.GetEndpoint();

                    if (endpoint?.Metadata.GetMetadata<IRouteNameMetadata>()?.RouteName != "SignOutEndpoint")
                    {
                        // Swap to a different endpoint entirely
                        var endpointDataSource = context.RequestServices
                            .GetRequiredService<EndpointDataSource>();

                        var newEndpoint = endpointDataSource.Endpoints
                            .FirstOrDefault(e => e.DisplayName == "SignOutEndpoint");

                        if (newEndpoint is not null)
                            context.SetEndpoint(newEndpoint); // ← router will now execute this one
                    }
                }
                else
                {
                    // Attach token to the request context so downstream handlers can use it
                    context.Items["access_token"] = tokenData.AccessToken;
                }
            }

            await _next(context);
        }
    }
}
