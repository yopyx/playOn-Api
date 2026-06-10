using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.VideoServices;

namespace ServiceLayer.CommentServices
{
    public class CommentDto
    {
        public string Id { get; set; } = string.Empty;
        public string TextOriginal { get; set; } = string.Empty;
        public string AuthorDisplayName { get; set; } = string.Empty;
        public string AuthorProfileImageUrl { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int TotalReplyCount { get; set; }
        public string PublishedAt { get; set; } = string.Empty;
        public string ListEtag {  get; set; } = string.Empty;
    }
    public class CommentsListDto
    {
        public IQueryable<CommentDto> CommentsList { get; set; } = Array.Empty<CommentDto>().AsQueryable();
        public bool QueryNextPage { get; set; }
    }

    public class CommentsListDtoEnumerable
    {
        public IEnumerable<CommentDto> CommentsList { get; set; } = Array.Empty<CommentDto>().AsQueryable();
        public bool QueryNextPage { get; set; }
    }
}
