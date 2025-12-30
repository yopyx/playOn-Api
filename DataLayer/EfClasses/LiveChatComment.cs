using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class LiveChatComment
    {
        public int LiveChatCommentId { get; set; }
        public string CId { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
        public string DisplayMessage { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string ListEtag { get; set; } = string.Empty;
        public string NextToken { get; set; } = string.Empty;


        //Relationships

        public int VideoWatchedId { get; set; }
        public VideoWatched VideoWatched { get; set; } = null!;  // navigation property
    }
}
