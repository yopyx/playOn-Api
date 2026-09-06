using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.DataTypes;

namespace DataLayer.EfClasses
{
    public class SearchResultedVideo
    {
        public int SearchResultedVideoId { get; set; }
        public string VId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
        public Dictionary<string, ThumbnailDetails> Thumbnails { get; set; } = [];
        public string[] Tags { get; set; } = [];
        public string LiveBroadcastContent { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Dictionary<string, string> Statistics { get; set; } = [];
        public Dictionary<string, string> LiveStreamingDetails { get; set; } = [];
        public string Etag { get; set; } = string.Empty;
        public string ListEtag { get; set; } = string.Empty;
        public string? NextToken { get; set; }
        public int PageNum { get; set; }

        //Relationships
        public int ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;  // navigation property
        public int SearchKeyId { get; set; }
        public SearchKey SearchKey { get; set; } = null!;  // navigation property

    }
}
