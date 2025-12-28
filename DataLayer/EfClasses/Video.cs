using static Shared.DataTypes;

namespace DataLayer.EfClasses
{
    public class Video
    {
        public int VideoId { get; set; }
        public string VId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
        public Dictionary<string, ThumbnailDetails>? Thumbnails { get; set; } = [];
        public string[] Tags { get; set; } = [];
        public string LiveBroadcastContent { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Dictionary<string, string>? Statistics { get; set; } = [];
        public Dictionary<string, string>? LiveStreamingDetails { get; set; }
        public string ListEtag { get; set; } = string.Empty;
        public string NextToken { get; set; } = string.Empty;


        //Relationships
        public int ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;  // navigation property
        public int VideoCategoryId { get; set; }
        public VideoCategory VideoCategory { get; set; } = null!;  // navigation property
    }
}
