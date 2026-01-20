using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.CommentServices;
using ServiceLayer.CommentServices.QueryObjects;
using ServiceLayer.VideoServices;
using DataLayer.QueryObjects;

namespace playOn.Endpoints
{
    public static class NestedCommentsEndpoints
    {
        public static void MapNestedCommentsEndpoints(this WebApplication app)
        {
            app.MapGet("/replies", async (EfCoreContext db, string commentId) =>
            {
                //Database Query
                var commentEntity = await db.Comments.SingleOrDefaultAsync(c => c.CId==commentId);
                if (commentEntity != null && commentEntity.TotalReplyCount > 0)
                {
                    var commentsList = db.Comments
                        .Where(c => c.CId == commentId)
                        .SelectMany(c => c.NestedComments)
                        .MapNestedCommentEntityToDto();
                    return TypedResults.Ok(commentsList);
                }
                return Results.Problem(statusCode: 404);
            });
        }
    }
}
