using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using static Shared.DataTypes;

namespace DataLayer.QueryObjects
{
    public static class ChangeComments
    {
        public static async Task AddCommentsToEntityAsync(this EfCoreContext db, CommentFromApi[] comments, VideoWatched video,string listEtag, string? token) {
            var entities = comments.Select(c => new Comment
            {
                CId = c.Id,
                TextOriginal = c.Snippet.TopLevelComment.Snippet.TextOriginal,
                AuthorDisplayName = c.Snippet.TopLevelComment.Snippet.AuthorDisplayName,
                AuthorProfileImageUrl = c.Snippet.TopLevelComment.Snippet.AuthorProfileImageUrl,
                LikeCount = c.Snippet.TopLevelComment.Snippet.LikeCount,
                TotalReplyCount = c.Snippet.TotalReplyCount,
                PublishedAt = c.Snippet.TopLevelComment.Snippet.PublishedAt,
                ListEtag = listEtag,
                NextToken = token,
                VideoWatched = video,
                NestedComments = c?.Replies==null? []: [.. c.Replies.Comments.Select(n => new NestedComment {
                    CId = n.Id,
                    TextOriginal = n.Snippet.TextOriginal,
                    AuthorDisplayName = n.Snippet.AuthorDisplayName,
                    AuthorProfileImageUrl = n.Snippet.AuthorProfileImageUrl,
                    LikeCount = n.Snippet.LikeCount,
                    ParentId = c.Id,
                    PublishedAt = n.Snippet.PublishedAt,
                })],
                
            });

            await db.Comments.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }
    }
}
