using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public static async Task AddApiDataToWatchedEntityAsync(this EfCoreContext db,IQueryable<Channel> channel, VideoDataFromApi v, string sub,User? user)
        {
            var defaultChannel = await db.Channels.FirstOrDefaultAsync(c => c.ChannelId == 1);

            var entity = new VideoWatched
            {
                VId = v.Id,
                Title = v.Snippet.Title,
                Description = v.Snippet.Description,
                PublishedAt = v.Snippet.PublishedAt,
                Thumbnails = v.Snippet.Thumbnails,
                Tags = v.Snippet.Tags ?? [],
                LiveBroadcastContent = v.Snippet.LiveBroadcastContent,
                Duration = v.ContentDetails.Duration,
                Statistics = v.Statistics,
                LiveStreamingDetails = v.LiveStreamingDetails ?? [],
                Etag = v.Etag,
                Channel = channel.FirstOrDefault()??defaultChannel!,
            };
            await db.VideosWatched.AddAsync(entity);
            await db.AddWatchedEntityToUserHistoryAsync(user,entity,sub);
            await db.SaveChangesAsync();
        }
        public static async Task AddVideoToWatchedEntityAsync(this EfCoreContext db, Video v,string sub, User? user) {
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
            await db.AddWatchedEntityToUserHistoryAsync(user,entity,sub);
            await db.SaveChangesAsync();
        }

        public static async Task AddResultToWatchedEntityAsync(this EfCoreContext db, SearchResultedVideo v,string sub, User? user)
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
            await db.AddWatchedEntityToUserHistoryAsync(user, entity,sub);
            await db.SaveChangesAsync();
        }
    }
}
