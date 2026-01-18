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

    }
}
