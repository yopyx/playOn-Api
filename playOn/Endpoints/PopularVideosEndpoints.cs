using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            app.MapGet("/recommended_videos", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, string videoCategoryId, string pageNum) => {

                //Database Query
                var page = Convert.ToInt16(pageNum);
                var categoryEntity = await db.VideoCategories.FirstOrDefaultAsync(c => c.VCId == videoCategoryId);
                if (categoryEntity==null)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of that category are not found",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }
                if (categoryEntity.PageNum >= page) {
                    var videoEntities = db.Videos
                        .Where(v => v.VideoCategory.VCId == videoCategoryId && v.PageNum == page)
                        .AsNoTracking()
                        .OrderBy(v => v.VideoId)
                        .Include(v => v.Channel);
                    var videoDtos = videoEntities.MapVideoEntityToDto();
                    var entity = await videoEntities.FirstOrDefaultAsync();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoDtos, QueryNextPage = entity?.NextToken != null });
                }

                //API Call
                var lastEntity = await db.Videos.Where(v => v.VideoCategory.VCId == videoCategoryId).OrderByDescending(v => v.VideoId).FirstOrDefaultAsync();
                var nextToken = lastEntity?.NextToken?? null;
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&maxResults=12{(nextToken != null ? "&pageToken=" + nextToken : "")}&regionCode=US&videoCategoryId={videoCategoryId}&key={apiKey}");
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
                var videoCards = json.Items.DistinctBy(v => v.Id).ToArray();
                //Extracting channels
                var channelIds = json.Items.Select(v => v.Snippet.ChannelId).Distinct();
                var channels = await helpers.GetChannelsAsync(channelIds); 


                //Update Database
                if (videoCards.Length > 0)
                {
                    try {
                        await db.UpdateCategoriesEntitiesAsync(categoryEntity);
                        await db.AddVideosToEntitiesAsync(videoCards, channels, json.Etag, json?.NextPageToken, categoryEntity);
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        // Someone else already wrote this — not a real error, just detach and re-read.
                        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                            entry.State = EntityState.Detached;
                    }
                }

                var videoCardsToDto = channels.MapVideoCardToDto(db, videoCards, json!.Etag);
                var data = new VideoCardListDto { VideoCardList = videoCardsToDto, QueryNextPage = json.NextPageToken != null };
                return TypedResults.Ok(data);
            });
        }
    }
}
