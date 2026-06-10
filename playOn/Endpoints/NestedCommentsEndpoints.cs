using System.Linq;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.CommentServices;
using ServiceLayer.CommentServices.QueryObjects;
using ServiceLayer.VideoServices;

namespace playOn.Endpoints
{
    public static class NestedCommentsEndpoints
    {
        public static void MapNestedCommentsEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/replies", [Authorize(policy: "ApiUser")] async (EfCoreContext db, string commentId) =>
            {
                //Database Query
                var commentEntity = await db.Comments.SingleOrDefaultAsync(c => c.CId==commentId);
                if (commentEntity != null && commentEntity.TotalReplyCount > 0)
                {
                    var commentsList = db.Comments
                        .AsNoTracking()
                        .Where(c => c.CId == commentId)
                        .SelectMany(c => c.NestedComments)
                        .MapNestedCommentEntityToDto();
                    return TypedResults.Ok(commentsList);
                }
                return Results.Problem(
                        title: "Not Found",
                        detail: "The replies are not found",
                        statusCode: 404
                    );
            });
        }
    }
}
