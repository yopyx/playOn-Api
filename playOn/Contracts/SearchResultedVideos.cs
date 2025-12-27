using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class Id
    {
        public string Kind {  get; set; } = string.Empty;
        public string VideoId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
    }
    public class Snippet3
    {
        public string PublishedAt { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, ThumbnailDetails> Thumbnails { get; set; } = [];
        public string ChannelTitle { get; set; } = string.Empty;
        public string LiveBroadcastContent { get; set; } = string.Empty;
        public string PublishTime { get; set; } = string.Empty;
    }
    public class Item2
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public Id Id { get; set; } = new();
        public Snippet3 Snippet { get; set; } = new();
    }
    public class SearchResultedVideos
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public string? NextPageToken { get; set; } = string.Empty;
        public string? PrevPageToken { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public PageInfo PageInfo { get; set; } = new();
        public Item2[] Items { get; set; } = [];
    }
}
