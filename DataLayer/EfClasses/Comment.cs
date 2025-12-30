using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string CId { get; set; } = string.Empty;
        public string TextOriginal { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;
        public string AuthorProfileImageUrl { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public string PublishedAt { get; set; } = string.Empty;
        public string ListEtag { get; set; } = string.Empty;
        public string NextToken { get; set; } = string.Empty;


        //Relationships

        public int VideoWatchedId { get; set; }
        public VideoWatched VideoWatched { get; set; } = null!;  // navigation property
    }
}