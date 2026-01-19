using System;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using playOn.Endpoints;
using playOn.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Registering ChannelHelpers class
builder.Services.AddScoped<IChannelHelpers, ChannelHelpers>();

// Registering VideoHelpers class
builder.Services.AddScoped<IVideoHelpers, VideoHelpers>();

var app = builder.Build();



if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    builder.Configuration.AddUserSecrets<Program>();
}

app.UseHttpsRedirection();


app.MapVideoCategoriesEndpoints();
app.MapPopularVideosEndpoints();
app.MapSearchSuggestionsEndpoints();
app.Run();
