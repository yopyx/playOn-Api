using System.Linq;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
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
        public static void MapLiveChatEndpoints(this WebApplication app)
        {
            app.MapGet("/liveChat", async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, string videoId, string pageNum) =>
            {

                //Database Query
                var watchedEntity = await db.VideosWatched.SingleOrDefaultAsync(v => v.VId == videoId);
                if(watchedEntity is null || watchedEntity?.LiveStreamingDetails?["activeLiveChatId"] is null)
                {
                    return Results.Problem(statusCode: 404);
                }
                var commentQuery = db.LiveComments.Where(c => c.VideoWatched.VId == videoId &&
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
                    return Results.Problem($"API call failed: {response.StatusCode}");
                }
                var json = await response.Content.ReadFromJsonAsync<LiveChat>();
            });
        }
    }
}
