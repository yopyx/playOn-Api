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
                var nextToken = await commentQuery.OrderByDescending(c => c.CommentId).Select(c => c.NextToken).FirstOrDefaultAsync();
                if (count >= Convert.ToInt16(pageNum) * 20)
                {
                    var commentsList = commentQuery
                        .AsNoTracking()
                        .OrderBy(c => c.CommentId)
                        .Skip((Convert.ToInt16(pageNum) - 1) * 20)
                        .Take(20)
                        .MapCommentEntityToDto();
                    return TypedResults.Ok(new CommentsListDto { CommentsList = commentsList, QueryNextPage = nextToken != null });
                }

                //API Call
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
                    await db.AddCommentsToEntityAsync(json!.Items, watchedEntity, json.Etag, json?.NextPageToken);
                }

                //Mapping json.Items to Dto
                var comments = json!.Items.AsQueryable().MapCommentToDto(json.Etag);
                return TypedResults.Ok(new CommentsListDto { CommentsList = comments, QueryNextPage = json?.NextPageToken != null });
            });
        }
    }
}
