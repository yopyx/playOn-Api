using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using playOn.Contracts;
using playOn.Helpers;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Endpoints
{
    public static class VideoWatchedEndpoints
    {
        public static void MapVideoWatchedEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/watch_video", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, HttpContext http, string videoId) =>
            {
                //Extracting sub from claims principle
                var sub = http.User.FindFirst("sub")?.Value;
                var user = await db.Users
                    .Include(u => u.HistoryRecords)
                    .ThenInclude(h => h.VideoWatchedLinks)
                    .ThenInclude(hv => hv.VideoWatched)
                    .ThenInclude(v => v.Channel)
                    .FirstOrDefaultAsync(u => u.UId == sub);
                //Database Query
                var videoWatched = await db.VideosWatched.Include(v => v.Channel).FirstOrDefaultAsync(v => v.VId == videoId);
                if (videoWatched != null)
                {
                    try
                    {
                        await db.AddWatchedEntityToUserHistoryAsync(user, videoWatched, sub!);
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        // Someone else already wrote this — not a real error, just detach and re-read.
                        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                            entry.State = EntityState.Detached;
                    }
                    return TypedResults.Ok(videoWatched.MapOneWatchedEntityToDto(null));
                }

                var video = await db.Videos.Include(v => v.Channel).FirstOrDefaultAsync(v => v.VId == videoId);
                if (video != null)
                {
                    //Add video to VideoWatched table
                    try
                    {
                        await db.AddVideoToWatchedEntityAsync(video, sub!, user);
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        // Someone else already wrote this — not a real error, just detach and re-read.
                        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                            entry.State = EntityState.Detached;
                    }
                    return TypedResults.Ok(video.MapOneVideoEntityToDto());
                }
                var searchResult = await db.VideoResults.Include(v => v.Channel).FirstOrDefaultAsync(v => v.VId == videoId);
                if (searchResult != null)
                {
                    //Add video result to VideoWatched table
                    try
                    {
                        await db.AddResultToWatchedEntityAsync(searchResult, sub!, user);
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        // Someone else already wrote this — not a real error, just detach and re-read.
                        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                            entry.State = EntityState.Detached;
                    }
                    return TypedResults.Ok(searchResult.MapOneResultEntityToDto());
                }


                //API Call for fetching videos ids
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics%2CliveStreamingDetails&id={videoId}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The video of that id is not found",
                        statusCode: 404
                    );
                }
                var json = await response.Content.ReadFromJsonAsync<VideosList>();
                if (json?.Items is null||json.Items.Length==0)
                {
                    //var emptyValue = Array.Empty<VideoCardDto>();
                    //return TypedResults.Ok(emptyValue);
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The video of that id is not found",
                        statusCode: 404
                    );
                }

                //Extracting channels
                var channelId = json.Items[0].Snippet.ChannelId;
                var channelsList = await helpers.GetChannelsAsync([channelId]);

                //Updating Database
                try
                {
                    await db.AddApiDataToWatchedEntityAsync(channelsList, json.Items[0], sub!, user);
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    // Someone else already wrote this — not a real error, just detach and re-read.
                    foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                        entry.State = EntityState.Detached;
                }
                var videoCardsToDto = channelsList.MapVideoCardToDto(db,json.Items, json.Etag).First();
                return TypedResults.Ok(videoCardsToDto);
            });
        }
    }
}
