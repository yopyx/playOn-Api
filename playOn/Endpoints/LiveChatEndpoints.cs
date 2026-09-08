using System.Linq;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using playOn.Contracts;
using ServiceLayer.CommentServices;
using ServiceLayer.CommentServices.QueryObjects;
using ServiceLayer.VideoServices;

namespace playOn.Endpoints
{
    public static class LiveChatEndpoints
    {
        public static void MapLiveChatEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/liveChat", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, string videoId, string pageNum) =>
            {

                //Database Query
                var watchedEntity = await db.VideosWatched.FirstOrDefaultAsync(v => v.VId == videoId);
                
                if(watchedEntity is null || watchedEntity?.LiveStreamingDetails["activeLiveChatId"] is null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The livechat is not found",
                        statusCode: 404
                    );
                }
                var page = Convert.ToInt32(pageNum);
                var commentQuery = db.LiveComments.Where(c => c.VideoWatched.VId == videoId);
                var lastEntity = await commentQuery.OrderByDescending(c => c.LiveChatCommentId).FirstOrDefaultAsync();
                var nextToken = lastEntity?.NextToken;
                if (page <= lastEntity?.PageNum)
                {
                    var liveCommentDtos = commentQuery.AsNoTracking()
                        .Where(c => c.PageNum == page)
                        .OrderBy(c => c.LiveChatCommentId)
                        .MapLiveCommentEntityToDto();

                    return TypedResults.Ok(new LiveCommentListDto { LiveCommentList = liveCommentDtos, QueryNextPage = nextToken!=null, PollingIntervalMillis= lastEntity?.PollingIntervalMillis });
                }

                //API Call
                var chatId = watchedEntity!.LiveStreamingDetails["activeLiveChatId"];
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"liveChat/messages?liveChatId={chatId}{(nextToken != null ? "&pageToken=" + nextToken : "")}&part=snippet%2CauthorDetails&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The livechat is not found",
                        statusCode: 404
                    );
                }
                var json = await response.Content.ReadFromJsonAsync<LiveChat>();
                if (json?.Items is null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The livechat is not found",
                        statusCode: 404
                    );
                }

                //Database Update
                try
                {
                    if (json!.Items.Length > 0)
                    {
                        await db.AddLiveCommentsToEntityAsync(json!.Items, watchedEntity, [videoId, json!.Etag, pageNum], json?.NextPageToken, json?.PollingIntervalMillis);
                    }
                    else if (nextToken != null && json!.Items.Length == 0 && json?.NextPageToken != null)
                    {
                        await db.UpdateLiveCommentsTokenAsync(nextToken, json.NextPageToken,json.PollingIntervalMillis);
                    }
                }
                // unique constraint violation
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    // Someone else already wrote this — not a real error, just detach and re-read.
                    foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                        entry.State = EntityState.Detached;
                }

                //Mapping json.Items to Dto
                var comments = json!.Items.AsQueryable().MapLiveCommentToDto(json.Etag);
                var data = new LiveCommentListDto { LiveCommentList = comments, QueryNextPage = json?.NextPageToken!=null, PollingIntervalMillis = json?.PollingIntervalMillis};
                return TypedResults.Ok(data);
            });
        }
    }
}
