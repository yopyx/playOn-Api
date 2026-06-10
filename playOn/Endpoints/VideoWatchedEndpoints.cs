using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                    .SingleOrDefaultAsync(u => u.UId == sub);

                //Database Query
                var videoWatched = await db.VideosWatched.Include(v => v.Channel).SingleOrDefaultAsync(v => v.VId == videoId);
                if (videoWatched != null)
                {
                    await db.AddWatchedEntityToUserHistoryAsync(user, videoWatched, sub!);
                    return TypedResults.Ok(videoWatched.MapOneWatchedEntityToDto(null));
                }

                var video = await db.Videos.Include(v => v.Channel).FirstOrDefaultAsync(v => v.VId == videoId);
                if (video != null)
                {
                    //Add video to VideoWatched table
                    await db.AddVideoToWatchedEntityAsync(video,sub!,user);

                    return TypedResults.Ok(video.MapOneVideoEntityToDto());
                }
                var searchResult = await db.VideoResults.Include(v => v.Channel).FirstOrDefaultAsync(v => v.VId == videoId);
                if (searchResult != null)
                {
                    //Add video result to VideoWatched table
                    await db.AddResultToWatchedEntityAsync(searchResult,sub!,user);

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
                await db.AddApiDataToWatchedEntityAsync(channelsList, json.Items[0], sub!,user);

                var videoCardsToDto = channelsList.MapVideoCardToDto(db,json.Items, json.Etag).First();
                return TypedResults.Ok(videoCardsToDto);
            });
        }
    }
}
