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
            });
        }
    }
}
