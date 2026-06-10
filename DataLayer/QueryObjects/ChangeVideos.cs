using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using static Shared.DataTypes;
using static Shared.Dtos;

namespace DataLayer.QueryObjects
{
    public static class ChangeVideos
    {
        public static async Task AddVideosToEntityAsync(this EfCoreContext db, VideoDataFromApi[] videos, VideoCategory videoCategory, IQueryable<Channel> channels, string listEtag, string? token) {
            var hashMap = new Dictionary<string, Channel>();
            foreach (var c in channels)
            {
                hashMap[c.ChId] = c;
            }
            var defaultChannel = await db.Channels.SingleOrDefaultAsync(c => c.ChannelId == 1);
            var entities = videos.Select(v => new Video
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
                ListEtag = listEtag,
                NextToken = token,
                Channel = hashMap.TryGetValue(v.Snippet.ChannelId, out var value) ? value: defaultChannel!,
                VideoCategory = videoCategory,
            });

            await db.Videos.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }
    }
}
