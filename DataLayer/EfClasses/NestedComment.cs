using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class NestedComment
    {
        public int NestedCommentId { get; set; }
        public string CId { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public string TextOriginal { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;
        public string AuthorProfileImageUrl { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public string PublishedAt { get; set; } = string.Empty;


        //Relationships

        public int CommentId { get; set; }
        public Comment Comment { get; set; } = null!;  // navigation property
    }
}
