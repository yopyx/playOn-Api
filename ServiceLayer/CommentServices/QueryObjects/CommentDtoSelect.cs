using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using static Shared.DataTypes;

namespace ServiceLayer.CommentServices.QueryObjects
{
    public static class CommentDtoSelect
    {
        public static IQueryable<CommentDto> MapCommentEntityToDto(this IQueryable<Comment> comments) {
            return comments.Select(c => new CommentDto {
                Id = c.CId,
                TextOriginal = c.TextOriginal,
                AuthorDisplayName = c.AuthorDisplayName,
                AuthorProfileImageUrl = c.AuthorProfileImageUrl,
                LikeCount = c.LikeCount,
                TotalReplyCount = c.TotalReplyCount,
                PublishedAt = c.PublishedAt,
                ListEtag = c.ListEtag,
            });
        }
    }
}
