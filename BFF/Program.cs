using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BFF;
using BFF.AuthEndpoints;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static BFF.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register the EF Core context for Data Protection key storage
builder.Services.AddDbContext<EfCoreContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Install: dotnet add package Microsoft.AspNetCore.DataProtection.EntityFrameworkCore
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<EfCoreContext>() // keys stored in SQL Server
    .SetApplicationName("playOn-bff");      // must match across restarts

//builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "PlayOn:"; // optional key prefix, useful if sharing one Redis DB across services
});

builder.Services.AddSession(options => {
    options.Cookie.Name = "session_id";
    options.Cookie.IsEssential = true; // Required for GDPR compliance if the session is essential for the app to function
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;    //lax    // Required for external OIDC sign-in
    options.IdleTimeout = TimeSpan.FromHours(1);   // was TimeSpan.FromMinutes(30);

});

// *** Authentication configuration ***
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // "Cookies"
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // "OpenIdConnect"
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "auth_cookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;        // Required for external OIDC sign-in
    options.ExpireTimeSpan = TimeSpan.FromHours(1); // Session cookie lifetime _ was TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;                  // Renew cookie if nearly expired
    //options.LoginPath = "/";

    options.Events = new CookieAuthenticationEvents
    {
        // If you need the authentication to fail immediately when a session is lost (e.g., if the server restarts and session data is gone)
        //OnValidatePrincipal = context =>
        //{
        //    var http = context.HttpContext;
        //    if (http.Session.Keys.IsNullOrEmpty())
        //    {
        //        context.RejectPrincipal();
        //    }
        //    return Task.CompletedTask;
        //},
        // Override the default login redirect for API calls by returning 401 instead of a 302, to avoid automatically redirecting AJAX calls.
        OnRedirectToLogin = context =>
        {
            // Prevent redirect loops in API calls
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
       
    };

}).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // options.SignInScheme if not explicitly set, it defaults to the DefaultSignInScheme or DefaultScheme configured in AddAuthentication
    // Same goes for options.SignOutScheme

    options.Authority = builder.Configuration["OIDC:Authority"];
    options.ClientId = builder.Configuration["OIDC:ClientId"];
    options.ClientSecret = builder.Configuration["OIDC:ClientSecret"];

    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UsePkce = true;
    // This automatically stores the access, refresh, and ID tokens inside the encrypted authentication ticket in the cookie
    //options.SaveTokens = true;

    if (builder.Environment.IsDevelopment())
    {
        options.RequireHttpsMetadata = false;
        IdentityModelEventSource.ShowPII = true;
    }

    // By default 'openid' and 'profile' are added
    // OpenID Connect scope for add user roles to the access token
    // for refresh tokens
    //options.Scope.Add("offline_access");

    // Redirect URIs (callback) set via Client settings
    // options.CallbackPath if not specified, it defaults to /signin-oidc
    // options.SignedOutCallbackPath if not specified, it defaults to /signout-callback-oidc
    //options.CallbackPath = "/signin-oidc";
    //options.SignedOutCallbackPath = "/signout-callback-oidc";

    options.GetClaimsFromUserInfoEndpoint = true;


    //keep claims from the ID token or UserInfo endpoint in their original format.
    options.MapInboundClaims = false;
    //options.TokenValidationParameters = new TokenValidationParameters
    //{
    //    RoleClaimType = "realm_access_roles"
    //};
    options.TokenValidationParameters.NameClaimType = "preferred_username";
    options.TokenValidationParameters.RoleClaimType = "realm_access_roles";

    // Set events to handle errors or token events if needed
    options.Events = new OpenIdConnectEvents
    {
        OnTokenResponseReceived = async context =>
        {
            // ? This Set() call is what actually causes the session cookie
            // to be written into the redirect response back to the browser
            context.HttpContext.Session.SetString("bff:active", "1");

            var tokenStore = context.HttpContext.RequestServices
                .GetRequiredService<ITokenStore>();

            // Use the session ID as the store key
            await context.HttpContext.Session.LoadAsync();

            // getting sessionId
            var sessionId = context.HttpContext.Session.Id; // sessionId is unique per user session, but it can change after expiry and re-login, so we don't use it as the key to store tokens in the distributed cache
            var rawIdTokenStr = context.TokenEndpointResponse.IdToken;

            // Store id_token for logout hint
            context.HttpContext.Session.SetString(
                "id_token",
                rawIdTokenStr  // ? the raw id_token string
            );

            // pull "sub" straight off the raw id_token.
            // OnTokenValidated hasn't run yet at this point in the pipeline, so context.Principal isn't populated —
            // reading the JWT directly is the reliable way to get a stable id this early.
            var idToken = new JwtSecurityTokenHandler().ReadJwtToken(rawIdTokenStr);
            var stableUserId = idToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(stableUserId)) return;  // spec-compliant IdP always sends this, but don't store under null
            
            var expiresIn = int.TryParse(context.TokenEndpointResponse.ExpiresIn, out var seconds)
                ? seconds : 3600;

            // we used stableUserId(instead of sessionId-which can be changed after expiry-) as the key to store the tokens
            // in the distributed cache, so that we can retrieve them later for token refresh
            await tokenStore.SaveAsync(stableUserId, new TokenData(
                AccessToken: context.TokenEndpointResponse.AccessToken,
                RefreshToken: context.TokenEndpointResponse.RefreshToken,
                IdToken: rawIdTokenStr,
                ExpiresAt: DateTimeOffset.UtcNow.AddSeconds(expiresIn),
                LastIntrospectedAt: DateTimeOffset.UtcNow));
        },
        OnRedirectToIdentityProvider = context =>
        {
            if(context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/user_profile"))
            {
                // Prevent redirect loops in API calls
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.HandleResponse(); // Suppress the redirect
            }
            return Task.CompletedTask;
        },
        OnRedirectToIdentityProviderForSignOut = async context =>
        {
            var tokenStore = context.HttpContext.RequestServices
                .GetRequiredService<ITokenStore>();

            // Use the session ID as the store key
            await context.HttpContext.Session.LoadAsync();
            var sessionId = context.HttpContext.Session.Id; // sessionId is unique per user session, but it can change after expiry and re-login, so we don't use it as the key to store tokens in the distributed cache
            var stableUserId = context.HttpContext.User.FindFirst("sub")?.Value;
            // Clean up tokens on logout
            if(!string.IsNullOrEmpty(stableUserId))
            {
                await tokenStore.RemoveAsync(stableUserId);
            }
        },

        OnTokenValidated = context => {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            // (ClaimTypes.Name) can replace (c => c.Type == "preferred_username") arrow function.
            var username = context.Principal?.FindFirst(c => c.Type == "preferred_username")?.Value;
            if(!string.IsNullOrEmpty(username))
            {
                // Add the preferred_username claim to the identity
                // "preferred_username" == ClaimTypes.Name that was configured in TokenValidationParameters.NameClaimType
                // but it is better to use the original claim type for clarity and maintainability.
                identity?.AddClaim(new Claim(ClaimTypes.Name, username ?? ""));
            }
            return Task.CompletedTask;
            // Now I can retrieve the username using context.User.Identity.Name / HttpContext.User in my application code.
        }

        //OnTokenValidated = context =>
        //{
        //    var http = context.HttpContext;
        //    var tokens = context.TokenEndpointResponse;
        //    if (tokens is null)
        //    {
        //        // Log the error and fail the authentication request
        //        context.Fail("Token endpoint response was null.");
        //        return Task.CompletedTask;
        //    }

        //    http.Session.SetString("access_token", tokens.AccessToken);
        //    http.Session.SetString("refresh_token", tokens.RefreshToken);
        //    http.Session.SetString("id_token", tokens.IdToken);

        //    http.Session.SetString("expires_at",
        //        DateTimeOffset.UtcNow.AddSeconds(int.Parse(tokens.ExpiresIn))
        //        .ToString("O"));
        //    return Task.CompletedTask;
        //}
    };
});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(@$"{builder.Configuration["ResourceApi:BaseUrl"]}/resources_api/");
});

builder.Services.AddHttpClient("KeycloakClient", client =>
{
    client.BaseAddress = new Uri(@$"{builder.Configuration["OIDC:Authority"]}/protocol/openid-connect");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientOnly", policy =>
    {
        policy.WithOrigins("http://localhost:5173",
    "http://127.0.0.1:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.AddScoped<ITokenStore, DistributedTokenStore>();
builder.Services.AddScoped<ITokenRefreshService, KeycloakTokenRefreshService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<EfCoreContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // log critical failure
        Console.WriteLine("Migration failed: " + ex.Message);
        throw; // optional
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseSession();
app.Use(async (ctx, next) =>
{
    await next();

    if (ctx.Response.StatusCode == 401)
    {
        var logger = ctx.RequestServices
            .GetRequiredService<ILogger<Program>>();

        logger.LogWarning(
            "401 on {Method} {Path} | Authenticated: {Auth} | Session keys: {Keys} | Cookies: {Cookies}",
            ctx.Request.Method,
            ctx.Request.Path,
            ctx.User.Identity?.IsAuthenticated,
            string.Join(", ", ctx.Session.Keys),
            string.Join(", ", ctx.Request.Cookies.Select(c => c.Key))
        );
    }
});

//app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("ClientOnly");
app.UseAuthentication();
app.UseMiddleware<TokenRefreshMiddleware>();
app.UseAuthorization();

// Auth APIs
app.MapSignInEndpoints();
app.MapSignOutEndpoints();
app.MapGet("/is_authenticated", (HttpContext ctx) =>
{
    if (ctx.User.Identity?.IsAuthenticated==false)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        name = ctx.User.Identity!.Name
    });
});

// Redirect APIs
app.MapLandingPagesEndpoints();

app.MapUserEndpoints();

var resourcesGroup = app.MapGroup("/api");
resourcesGroup.MapProtectedResourcesEndpoints();

app.MapGet("/debug/auth", async (HttpContext ctx) =>
{
    await ctx.Session.LoadAsync();

    var accessToken = ctx.Session.GetString("access_token");
    var refreshToken = ctx.Session.GetString("refresh_token");

    return Results.Ok(new
    {
        isAuthenticated = ctx.User.Identity?.IsAuthenticated,
        authType = ctx.User.Identity?.AuthenticationType,
        claims = ctx.User.Claims.Select(c => new { c.Type, c.Value }),
        sessionKeys = ctx.Session.Keys.ToList(),
        hasAccessToken = !string.IsNullOrEmpty(accessToken),
        hasRefreshToken = !string.IsNullOrEmpty(refreshToken),
        // decode without validation just to inspect
        accessTokenIss = accessToken != null
            ? System.IdentityModel.Tokens.Jwt.JwtPayload
                .Base64UrlDeserialize(accessToken.Split('.')[1])
                .TryGetValue("iss", out var iss) ? iss : "n/a"
            : "no token in session",
        cookies = ctx.Request.Cookies.Select(c => c.Key).ToList()
    });
}).AllowAnonymous();

app.Run();
