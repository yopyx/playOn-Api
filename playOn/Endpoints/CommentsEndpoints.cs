using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using ServiceLayer.CommentServices;
using ServiceLayer.CommentServices.QueryObjects;
using ServiceLayer.VideoServices;
using static Shared.DataTypes;

namespace playOn.Endpoints
{
    public static class CommentsEndpoints
    {
        public static void MapCommentsEndpoints(this WebApplication app)
        {
            app.MapGet("/comments", async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, string videoId, string pageNum) =>
            {
                //Database Query
                var watchedEntity = await db.VideosWatched.SingleOrDefaultAsync(v => v.VId == videoId);
                if (watchedEntity == null)
                {
                    return Results.Problem(statusCode: 404);
                }
                var commentQuery = db.Comments.Where(c => c.VideoWatched.VId == videoId);
                var count = await commentQuery.CountAsync();
                var nextToken = await commentQuery.OrderByDescending(c => c.CommentId).Select(c => c.NextToken).FirstOrDefaultAsync();
                if (count >= Convert.ToInt16(pageNum) * 20)
                {
                    var commentsList = commentQuery
                        .OrderBy(c => c.CommentId)
                        .Skip((Convert.ToInt16(pageNum) - 1) * 20)
                        .Take(20)
                        .MapCommentEntityToDto();
                    return TypedResults.Ok(new CommentsListDto { CommentsList = commentsList, QueryNextPage = nextToken != null });
                }
            });
        }
    }
}
