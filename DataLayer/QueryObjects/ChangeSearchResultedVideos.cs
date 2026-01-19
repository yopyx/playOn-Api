using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using static Shared.DataTypes;

namespace DataLayer.QueryObjects
{
    public static class ChangeSearchResultedVideos
    {
        public static async Task AddResultsToEntityAsync(this EfCoreContext db, VideoDataFromApi[] videos, IQueryable<Channel> channels, SearchKey searchKeyEntity)
        {
            var hashMap = new Dictionary<string, Channel>();
            foreach (var c in channels)
            {
                hashMap[c.ChId] = c;
            }
            var entities = videos.Select(v => new SearchResultedVideo
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
                ListEtag = searchKeyEntity.VideoListEtag,
                NextToken = searchKeyEntity.NextToken,
                Channel = hashMap[v.Snippet.ChannelId],
                SearchKey = searchKeyEntity,
            });

            await db.VideoResults.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }
    }
}
