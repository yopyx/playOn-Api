using System;
using System.Security.Cryptography;
using DataLayer.EfCode;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using playOn.Endpoints;
using playOn.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// *** Authentication configuration ***
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
        {
            options.Authority = builder.Configuration["OIDC:Authority"];

            if (builder.Environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
            }

            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["OIDC:Authority"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["OIDC:Audience"],
                //ValidAudience = builder.Configuration["OIDC:Audience"], // must match Keycloak audience (set on mapper)

                ValidateLifetime = true,

                NameClaimType = "preferred_username",
                RoleClaimType = "realm_access_roles"
            };
    });

// *** Authorization configuration ***
builder.Services.AddAuthorizationBuilder()
       .AddPolicy("AdminOnly", policy => {
            policy.RequireAuthenticatedUser().RequireRole("admin");
       })
       .AddPolicy("ApiUser", policy => {
            policy.RequireAuthenticatedUser().RequireRole("user");
       });

builder.Services.ConfigureHttpJsonOptions(o => {
    o.SerializerOptions.AllowTrailingCommas = true;
    o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    o.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddHttpClient("YouTubeClient", client =>
{
    client.BaseAddress = new Uri(@"https://youtube.googleapis.com/youtube/v3/");
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EfCoreContext>(options =>
{
    options.UseSqlServer(connection);
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}


// Registering ChannelHelpers class
builder.Services.AddScoped<IChannelHelpers, ChannelHelpers>();

// Registering VideoHelpers class
builder.Services.AddScoped<IVideoHelpers, VideoHelpers>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BFFOnly", policy =>
    {
        policy.WithOrigins("https://localhost:7117",
    "https://127.0.0.1:7117").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

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

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("BFFOnly");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();




// Protected API Resources
var resourceApi = app.MapGroup("/api");
resourceApi.MapVideoCategoriesEndpoints();
resourceApi.MapPopularVideosEndpoints();
resourceApi.MapSearchSuggestionsEndpoints();
resourceApi.MapSearchResultedVideosEndpoints();
resourceApi.MapVideoWatchedEndpoints();
resourceApi.MapCommentsEndpoints();
resourceApi.MapNestedCommentsEndpoints();
resourceApi.MapLiveChatEndpoints();
resourceApi.MapWatchHistoryEndpoints();

app.Run();
