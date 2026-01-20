using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.VideoServices;

namespace ServiceLayer.CommentServices
{
    public class LiveCommentDto
    {
        public string Id { get; set; } = string.Empty;
        public string PublishedAt { get; set; } = string.Empty;
        public string DisplayMessage { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string ListEtag { get; set; } = string.Empty;
    }

    public class LiveCommentListDto
    {
        public IQueryable<LiveCommentDto> LiveCommentList { get; set; } = Array.Empty<LiveCommentDto>().AsQueryable();
        public bool QueryNextPage { get; set; }
    }
}
