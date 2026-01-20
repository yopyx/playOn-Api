using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using static Shared.DataTypes;

namespace ServiceLayer.CommentServices.QueryObjects
{
    public static class LiveCommentDtoSelect
    {
        public static IQueryable<LiveCommentDto> MapLiveCommentEntityToDto(this IQueryable<LiveChatComment> comments)
        {
            return comments.Select(c => new LiveCommentDto
            {
                Id = c.CId,
                PublishedAt = c.PublishedAt,
                DisplayMessage = c.DisplayMessage,
                DisplayName = c.DisplayName,
                ProfileImageUrl = c.ProfileImageUrl,
                ListEtag = c.ListEtag,
            });
        }

        public static IQueryable<LiveCommentDto> MapLiveCommentToDto(this IQueryable<LiveCommentFromApi> comments, string listEtag)
        {
            return comments.Select(c => new LiveCommentDto
            {
                Id = c.Id,
                PublishedAt = c.Snippet.PublishedAt,
                DisplayMessage = c.Snippet.DisplayMessage,
                DisplayName = c.AuthorDetails.DisplayName,
                ProfileImageUrl = c.AuthorDetails.ProfileImageUrl,
                ListEtag = listEtag,
            });
        }
    }
}
