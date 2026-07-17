using System.Linq;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var watchedEntity = await db.VideosWatched.SingleOrDefaultAsync(v => v.VId == videoId);
                
                if(watchedEntity is null || watchedEntity?.LiveStreamingDetails["activeLiveChatId"] is null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The livechat is not found",
                        statusCode: 404
                    );
                }
                var commentQuery = db.LiveComments.Where(c => c.VideoWatched.VId == videoId);
                var lastEntity = await commentQuery.OrderByDescending(c => c.LiveChatCommentId).FirstOrDefaultAsync();
                var nextToken = lastEntity?.NextToken;
                if (commentQuery.Any() && pageNum=="1")
                {
                    var liveCommentDtos = commentQuery.AsNoTracking()
                        .OrderBy(c => c.LiveChatCommentId).MapLiveCommentEntityToDto();

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
                catch (DbUpdateException) // unique constraint violation
                {
                    db.ChangeTracker.Clear();
                }

                //Mapping json.Items to Dto
                var comments = json!.Items.AsQueryable().MapLiveCommentToDto(json.Etag);
                var data = new LiveCommentListDto { LiveCommentList = comments, QueryNextPage = json?.NextPageToken!=null, PollingIntervalMillis = json?.PollingIntervalMillis};
                return TypedResults.Ok(data);
            });
        }
    }
}
