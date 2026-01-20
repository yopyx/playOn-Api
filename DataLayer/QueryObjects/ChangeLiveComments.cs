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
    public static class ChangeLiveComments
    {
        public static async Task AddLiveCommentsToEntityAsync(this EfCoreContext db, LiveCommentFromApi[] comments, VideoWatched video,string[] props, string? token)
        {
            var entities = comments.Select(c => new LiveChatComment
            {
                CId = c.Id,
                PublishedAt = c.Snippet.PublishedAt,
                DisplayMessage = c.Snippet.DisplayMessage,
                DisplayName = c.AuthorDetails.DisplayName,
                ProfileImageUrl = c.AuthorDetails.ProfileImageUrl,
                ListEtag = props[1],
                PageNo = props[2],
                NextToken = token,
                VideoWatched = video,
            });

            await db.LiveComments.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }

        public static async Task UpdateLiveCommentsTokenAsync(this EfCoreContext db, string prevToken, string token)
        {
            var list = await db.LiveComments.Where(c => c.NextToken == prevToken).ToListAsync();
            foreach (var comment in list)
            {
                comment.NextToken = token;
            }
            await db.SaveChangesAsync();
        }
    }
}
