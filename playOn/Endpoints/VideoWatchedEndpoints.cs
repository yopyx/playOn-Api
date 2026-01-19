using System.Collections.Generic;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
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
        public static void MapVideoWatchedEndpoints(this WebApplication app)
        {
            app.MapGet("/watch_video", async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, string videoId) =>
            {
                //Database Query
                var videoWatched = await db.VideosWatched.SingleOrDefaultAsync(v => v.VId == videoId);
                if (videoWatched != null)
                {
                    return TypedResults.Ok(videoWatched.MapOneWatchedEntityToDto());
                }
                var video = await db.Videos.SingleOrDefaultAsync(v => v.VId == videoId);
                if (video != null)
                {
                    //Add video to VideoWatched table
                    await db.AddVideoToWatchedEntityAsync(video);

                    return TypedResults.Ok(video.MapOneVideoEntityToDto());
                }
                var searchResult = await db.VideoResults.SingleOrDefaultAsync(v => v.VId == videoId);
                if (searchResult != null)
                {
                    //Add video result to VideoWatched table
                    await db.AddResultToWatchedEntityAsync(searchResult);

                    return TypedResults.Ok(searchResult.MapOneResultEntityToDto());
                }


                //API Call for fetching videos ids
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics%2CliveStreamingDetails&id={videoId}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem($"API call failed: {response.StatusCode}");
                }
                var json = await response.Content.ReadFromJsonAsync<VideosList>();
                if (json?.Items is null||json.Items.Length==0)
                {
                    var emptyValue = Array.Empty<VideoCardDto>();
                    return TypedResults.Ok(emptyValue);
                }

                //Extracting channels
                var channelId = json.Items[0].Snippet.ChannelId;
                var channelsList = await helpers.GetChannelsAsync([channelId]);

                //Updating Database
                await db.AddApiDataToWatchedEntityAsync(channelsList, json.Items[0]);

                var videoCardsToDto = channelsList.MapVideoCardToDto(json.Items, json.Etag).First();
                return TypedResults.Ok(videoCardsToDto);
            });
        }
    }
}
