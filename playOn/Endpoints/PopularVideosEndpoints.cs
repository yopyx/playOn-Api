using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using playOn.Helpers;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Endpoints
{
    public static class PopularVideosEndpoints
    {
        public static void MapPopularVideosEndpoints(this WebApplication app)
        {
            app.MapGet("/recommended_videos", async(IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, string VideoCategoryId, string pageNum) => {

                //Database Query
                var videoQuery = db.Videos.Where(v => v.VideoCategory.VCId==VideoCategoryId);
                var count = await videoQuery.CountAsync();
                //var category = await db.VideoCategories.Include(v => v.Videos).SingleOrDefaultAsync(c => c.VCId == VideoCategoryId);
                var nextToken = await videoQuery.OrderByDescending(v => v.VideoId).Select(v => v.NextToken).FirstOrDefaultAsync();
                if (count >= Convert.ToInt16(pageNum) * 12) {
                    var videoEntities = videoQuery
                        .OrderBy(v => v.VideoId)
                        .Skip((Convert.ToInt16(pageNum) - 1) * 12)
                        .Take(12)
                        .MapVideoEntityToDto();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoEntities, QueryNextPage = nextToken != null });
                }

                //API Call
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&maxResults=12{(nextToken != null ? "&pageToken=" + nextToken : "")}&regionCode=US&videoCategoryId={VideoCategoryId}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem($"API call failed: {response.StatusCode}");
                }
                var json = await response.Content.ReadFromJsonAsync<VideosList>();
                
                if (json!.Items.Length==0)
                {
                    var emptyValue = Enumerable.Empty<VideoCardDto>().AsQueryable();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = emptyValue, QueryNextPage = false});
                }
            });
        }
    }
}
