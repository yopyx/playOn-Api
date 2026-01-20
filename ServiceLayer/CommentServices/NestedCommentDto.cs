using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.CommentServices
{
    public class NestedCommentDto
    {
        public string Id { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public string TextOriginal { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;
        public string AuthorProfileImageUrl { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public string PublishedAt { get; set; } = string.Empty;
    }
}