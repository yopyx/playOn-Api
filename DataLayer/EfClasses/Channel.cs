using static Shared.DataTypes;

namespace DataLayer.EfClasses
{
    public class Channel
    {
        public int ChannelId { get; set; }
        public string ChId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public Dictionary<string, ThumbnailDetails> Thumbnails { get; set; } = [];
        public Statistics Statistics { get; set; } = new();


        //Relationships
        public ICollection<Video> Videos { get; set; } = [];
        public ICollection<SearchResultedVideo> SearchResultedVideos { get; set; } = [];
    }
}
