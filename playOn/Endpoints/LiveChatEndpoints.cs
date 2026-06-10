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
                var commentQuery = db.LiveComments.AsNoTracking().Where(c => c.VideoWatched.VId == videoId &&
                c.PageNo == pageNum).OrderBy(c => c.LiveChatCommentId);
                //var commentListTokens = await commentQuery.Select(c => c.NextToken).Distinct().ToArrayAsync();
                var nextToken = commentQuery.Any()? commentQuery.First().NextToken :null;
                if (commentQuery.Any())
                {
                    var liveComments = commentQuery.MapLiveCommentEntityToDto();
                    
                    return TypedResults.Ok(new LiveCommentListDto { LiveCommentList = liveComments, QueryNextPage = nextToken != null });
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
                if (json?.Items is null || json.Items.Length == 0)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The livechat is not found",
                        statusCode: 404
                    );
                }

                //Database Update
                if (json!.Items.Length > 0) {
                    await db.AddLiveCommentsToEntityAsync(json!.Items, watchedEntity,[videoId, json!.Etag, pageNum], json?.NextPageToken);
                }else if (nextToken!=null && json!.Items.Length == 0 && json?.NextPageToken != null)
                {
                    await db.UpdateLiveCommentsTokenAsync(nextToken,json!.NextPageToken);
                }

                //Mapping json.Items to Dto
                var comments = json!.Items.AsQueryable().MapLiveCommentToDto(json.Etag);
                var data = new LiveCommentListDto { LiveCommentList = comments, QueryNextPage = json?.NextPageToken!=null };
                return TypedResults.Ok(data);
            });
        }
    }
}
