using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using static Shared.DataTypes;

namespace ServiceLayer.VideoServices.QueryObjects
{
    public static class VideoCardDtoSelect
    {
        public static IQueryable<VideoCardDto> MapVideoEntityToDto(this IQueryable<Video> videoCards)
        {
            return videoCards.Select(v => new VideoCardDto
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

        public static VideoCardDto MapOneWatchedEntityToDto(this VideoWatched v)
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
                Etag = v.Etag,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = v.Channel.Statistics!.SubscriberCount,
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

        public static IQueryable<VideoCardDto> MapVideoCardToDto(this IQueryable<Channel> channels, VideoDataFromApi[] videos, string listEtag) {
            var hashMap = new Dictionary<string, Channel>();
            foreach (var c in channels) {
                hashMap[c.ChId] = c;
            }
            return videos.Select(v => new VideoCardDto
            {
                VideoId = v.Id,
                ChannelId = v.Snippet.ChannelId,
                Title = v.Snippet.Title,
                ChannelTitle = hashMap[v.Snippet.ChannelId].Title,
                Description = v.Snippet.Description,
                PublishedAt = v.Snippet.PublishedAt,
                Thumbnails = v.Snippet.Thumbnails,
                ChannelThumbnails = hashMap[v.Snippet.ChannelId].Thumbnails,
                Tags = v.Snippet.Tags ?? [],
                Etag = v.Etag,
                ListEtag = listEtag,
                LiveBroadcastContent = v.Snippet.LiveBroadcastContent,
                Duration = v.ContentDetails.Duration,
                Statistics = v.Statistics,
                SubscriberCount = hashMap[v.Snippet.ChannelId].Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails ?? [],
            }).AsQueryable();
        }
    }
}
