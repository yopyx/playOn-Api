using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
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
        public static void MapPopularVideosEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/recommended_videos", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, string VideoCategoryId, string pageNum) => {

                //Database Query
                var videoQuery = db.Videos.Where(v => v.VideoCategory.VCId==VideoCategoryId);
                var count = await videoQuery.CountAsync();
                //var category = await db.VideoCategories.Include(v => v.Videos).SingleOrDefaultAsync(c => c.VCId == VideoCategoryId);
                var tokens = videoQuery.AsNoTracking().OrderBy(v => v.VideoId).Select(v => v.NextToken).Distinct().ToArray();
                var page = Convert.ToInt16(pageNum);
                if (count >= page * 12) {
                    var videoEntities = videoQuery
                        .AsNoTracking()
                        .OrderBy(v => v.VideoId)
                        .Skip((page - 1) * 12)
                        .Take(12)
                        .Include(v => v.Channel)
                        .MapVideoEntityToDto();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoEntities, QueryNextPage = tokens[page - 1] != null });
                }

                //API Call
                var nextToken = page > 1 ? tokens[^1] : null;
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&maxResults=12{(nextToken != null ? "&pageToken=" + nextToken : "")}&regionCode=US&videoCategoryId={VideoCategoryId}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    //return Results.Problem($"API call failed: {response.StatusCode}");
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of that category are not found",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }
                var json = await response.Content.ReadFromJsonAsync<VideosList>();
                
                if (json?.Items is null || json.Items.Length == 0)
                {
                    //var emptyValue = Enumerable.Empty<VideoCardDto>().AsQueryable();
                    //return TypedResults.Ok(new VideoCardListDto { VideoCardList = emptyValue, QueryNextPage = false});
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of that category are not found",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }

                //Extracting channels
                var channelIds = json.Items.Select(v => v.Snippet.ChannelId).Distinct();
                var channels = await helpers.GetChannelsAsync(channelIds); 

                var category = await db.VideoCategories.FirstOrDefaultAsync(c => c.VCId == VideoCategoryId);

                //Update Database
                if (json.Items.Length > 0)
                {
                    await db.AddVideosToEntityAsync(json.Items, category!, channels, json.Etag, json?.NextPageToken);
                }

                var videoCardsToDto = channels.MapVideoCardToDto(db,json!.Items, json.Etag);
                var data = new VideoCardListDto { VideoCardList = videoCardsToDto, QueryNextPage = json.NextPageToken != null };
                return TypedResults.Ok(data);
            });
        }
    }
}
