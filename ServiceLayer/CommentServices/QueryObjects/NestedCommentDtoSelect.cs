using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;

namespace ServiceLayer.CommentServices.QueryObjects
{
    public static class NestedCommentDtoSelect
    {
        public static IQueryable<NestedCommentDto> MapNestedCommentEntityToDto(this IQueryable<NestedComment> comments)
        {
            return comments.Select(c => new NestedCommentDto
            {
                Id = c.CId,
                ParentId = c.ParentId,
                TextOriginal = c.TextOriginal,
                AuthorDisplayName = c.AuthorDisplayName,
                AuthorProfileImageUrl = c.AuthorProfileImageUrl,
                LikeCount = c.LikeCount,
                PublishedAt = c.PublishedAt,
            });
        }
    }
}
