using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using static Shared.DataTypes;

namespace ServiceLayer.VideoServices.QueryObjects
{
    public static class VideoCardDtoSelect
    {
        public static IQueryable<VideoCardDto> MapVideoEntityToDto(this IQueryable<Video> videoCards)
        {
            return videoCards
                .Where(v => v.Channel.Title!= "-unknown_Channel-")
                .Select(v => new VideoCardDto
            {
                VideoId = v.VId,
                ChannelId = v.Channel.ChId,
                Title = v.Title,
                ChannelTitle = v.Channel.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                ChannelThumbnails = v.Channel.Thumbnails,
                Tags = v.Tags,
                ListEtag = v.ListEtag,
                Etag = v.Etag,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = v.Channel.Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails,
            });
        }

        public static VideoCardDto MapOneWatchedEntityToDto(this VideoWatched v, Channel? channel)
        {
            channel = channel ?? v.Channel;
            return new VideoCardDto
            {
                VideoId = v.VId,
                ChannelId = channel.ChId,
                Title = v.Title,
                ChannelTitle = channel.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                ChannelThumbnails = channel.Thumbnails,
                Tags = v.Tags,
                Etag = v.Etag,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = channel.Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails,
            };
        }
        public static VideoCardDto MapOneVideoEntityToDto(this Video v)
        {
            return new VideoCardDto
            {
                VideoId = v.VId,
                ChannelId = v.Channel.ChId,
                Title = v.Title,
                ChannelTitle = v.Channel.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                ChannelThumbnails = v.Channel.Thumbnails,
                Tags = v.Tags,
                ListEtag = v.ListEtag,
                Etag = v.Etag,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = v.Channel.Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails,
            };
        }

        public static VideoCardDto MapOneResultEntityToDto(this SearchResultedVideo v)
        {
            return new VideoCardDto
            {
                VideoId = v.VId,
                ChannelId = v.Channel.ChId,
                Title = v.Title,
                ChannelTitle = v.Channel.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                ChannelThumbnails = v.Channel.Thumbnails,
                Tags = v.Tags,
                ListEtag = v.ListEtag,
                Etag = v.Etag,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = v.Channel.Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails,
            };
        }

        public static IQueryable<VideoCardDto> MapVideoCardToDto(this IQueryable<Channel> channels, EfCoreContext db,VideoDataFromApi[] videos, string listEtag) {
            var hashMap = new Dictionary<string, Channel>();
            foreach (var c in channels) {
                hashMap[c.ChId] = c;
            }
            var defaultChannel = db.Channels.SingleOrDefault(c => c.ChannelId == 1);
            return videos.Select(v => new VideoCardDto
            {
                VideoId = v.Id,
                ChannelId = v.Snippet.ChannelId,
                Title = v.Snippet.Title,
                ChannelTitle = (hashMap.TryGetValue(v.Snippet.ChannelId, out var value) ? value : defaultChannel!).Title,
                Description = v.Snippet.Description,
                PublishedAt = v.Snippet.PublishedAt,
                Thumbnails = v.Snippet.Thumbnails,
                ChannelThumbnails = (hashMap.TryGetValue(v.Snippet.ChannelId, out var value2) ? value2 : defaultChannel!).Thumbnails,
                Tags = v.Snippet.Tags ?? [],
                Etag = v.Etag,
                ListEtag = listEtag,
                LiveBroadcastContent = v.Snippet.LiveBroadcastContent,
                Duration = v.ContentDetails.Duration,
                Statistics = v.Statistics,
                SubscriberCount = (hashMap.TryGetValue(v.Snippet.ChannelId, out var value3) ? value3 : defaultChannel!).Statistics!.SubscriberCount,
                LiveStreamingDetails = v?.LiveStreamingDetails ?? [],
            }).Where(v => v.ChannelTitle != "-unknown_Channel-").AsQueryable(); 
        }
    }
}
