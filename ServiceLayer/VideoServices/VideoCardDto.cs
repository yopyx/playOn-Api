using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.DataTypes;

namespace ServiceLayer.VideoServices
{
    public class VideoCardDto
    {
        public string VideoId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ChannelTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
        public Dictionary<string, ThumbnailDetails>? Thumbnails { get; set; } = [];
        public Dictionary<string, ThumbnailDetails>? ChannelThumbnails { get; set; } = [];
        public string[] Tags { get; set; } = [];
        public string ListEtag {  get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public string LiveBroadcastContent { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Dictionary<string, string> Statistics { get; set; } = [];
        public string SubscriberCount { get; set; } = string.Empty;
        public Dictionary<string, string> LiveStreamingDetails { get; set; } = [];
    }

    public class VideoCardListDto
    {
        public IQueryable<VideoCardDto> VideoCardList { get; set; } = Array.Empty<VideoCardDto>().AsQueryable();
        public bool QueryNextPage {  get; set; }
    }

    public class VideoCardListDtoEnumerable
    {
        public IEnumerable<VideoCardDto> VideoCardList { get; set; } = Array.Empty<VideoCardDto>().AsQueryable();
        public bool QueryNextPage { get; set; }
    }
}
