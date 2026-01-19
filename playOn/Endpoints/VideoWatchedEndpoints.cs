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
            });
        }
    }
}
