using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
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
        public static void MapCommentsEndpoints(this RouteGroupBuilder app) {
            app.MapGet("/comments", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, string videoId, string pageNum) =>
            {
                //Database Query
                var watchedEntity = await db.VideosWatched.SingleOrDefaultAsync(v => v.VId == videoId);
                if (watchedEntity == null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The video comments are not found",
                        statusCode: 404
                    );
                }
                var commentQuery = db.Comments.Where(c => c.VideoWatched.VId == videoId);
                var count = await commentQuery.CountAsync();
                
                if (count >= Convert.ToInt16(pageNum) * 20)
                {
                    var commentEntities = commentQuery
                        .AsNoTracking()
                        .OrderBy(c => c.CommentId)
                        .Skip((Convert.ToInt16(pageNum) - 1) * 20)
                        .Take(20);

                    var commentsDto = commentEntities.MapCommentEntityToDto();
                    var entity = await commentEntities.FirstOrDefaultAsync();
                    return TypedResults.Ok(new CommentsListDto { CommentsList = commentsDto, QueryNextPage = entity?.NextToken != null });
                }

                //API Call
                var lastEntity = await commentQuery.OrderByDescending(c => c.CommentId).FirstOrDefaultAsync();
                var nextToken = count > 0 ? lastEntity?.NextToken : null;
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"commentThreads?part=snippet%2Creplies{(nextToken != null ? "&pageToken=" + nextToken : "")}&videoId={videoId}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The video comments are not found",
                        statusCode: 404
                    );
                }
                var json = await response.Content.ReadFromJsonAsync<Comments>();
                if (json?.Items is null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The video comments are not found",
                        statusCode: 404
                    );
                }

                //Database Update
                if (json!.Items.Length > 0)
                {
                    try
                    {
                        await db.AddCommentsToEntityAsync(json!.Items, watchedEntity, json.Etag, json?.NextPageToken);
                    }
                    catch (DbUpdateException) // unique constraint violation
                    {
                        db.ChangeTracker.Clear();
                    }
                }

                //Mapping json.Items to Dto
                var comments = json!.Items.AsQueryable().MapCommentToDto(json.Etag);
                return TypedResults.Ok(new CommentsListDto { CommentsList = comments, QueryNextPage = json?.NextPageToken != null });
            });
        }
    }
}
