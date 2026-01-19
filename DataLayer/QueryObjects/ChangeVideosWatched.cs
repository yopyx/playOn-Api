using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using static Shared.DataTypes;

namespace DataLayer.QueryObjects
{
    public static class ChangeVideosWatched
    {
        public static async Task AddVideoToWatchedEntityAsync(this EfCoreContext db, Video v) {
            var entity = new VideoWatched
            {
                VId = v.VId,
                Title = v.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                Tags = v.Tags,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                LiveStreamingDetails = v.LiveStreamingDetails,
                Etag = v.Etag,
                Channel = v.Channel,
            };
            await db.VideosWatched.AddAsync(entity);
            await db.SaveChangesAsync();
        }

        public static async Task AddResultToWatchedEntityAsync(this EfCoreContext db, SearchResultedVideo v)
        {
            var entity = new VideoWatched
            {
                VId = v.VId,
                Title = v.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                Tags = v.Tags,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                LiveStreamingDetails = v.LiveStreamingDetails,
                Etag = v.Etag,
                Channel = v.Channel,
            };
            await db.VideosWatched.AddAsync(entity);
            await db.SaveChangesAsync();
        }
    }
}
