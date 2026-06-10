using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using ServiceLayer.VideoServices;

namespace ServiceLayer.SearchResultServices.QueryObjects
{
    public static class VideoResultDtoSelect
    {
        public static IQueryable<VideoCardDto> MapVideoResultEntityToDto(this IQueryable<SearchResultedVideo> videoResults)
        {
            return videoResults
                .Where(v => v.Channel.Title != "-unknown_Channel-")
                    .Select(v => new VideoCardDto
            {
                VideoId = v.VId,
                ChannelId = v.Channel.ChId,
                ListEtag = v.ListEtag,
                Etag = v.Etag,
                Title = v.Title,
                ChannelTitle = v.Channel.Title,
                Description = v.Description,
                PublishedAt = v.PublishedAt,
                Thumbnails = v.Thumbnails,
                ChannelThumbnails = v.Channel.Thumbnails,
                Tags = v.Tags,
                LiveBroadcastContent = v.LiveBroadcastContent,
                Duration = v.Duration,
                Statistics = v.Statistics,
                SubscriberCount = v.Channel.Statistics!.SubscriberCount,
                LiveStreamingDetails = v.LiveStreamingDetails,
            });
        }
    }
}
