using System.Collections.Generic;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using playOn.Contracts;
using playOn.Helpers;
using ServiceLayer.SearchResultServices.QueryObjects;
using ServiceLayer.VideoServices;


namespace playOn.Endpoints
{
    public static class SearchResultedVideosEndpoints
    {
        public static void MapSearchResultedVideosEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/search_results", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IVideoHelpers videohelpers, string q, string pageNum="1") =>
            {
                //Database Query
                q = q.Trim().ToLower();
                var keyEntity = await db.SearchKeys.SingleOrDefaultAsync(k => k.Title==q);
                var videoQuery = db.VideoResults.Where(v => v.SearchKey.Title == q);
                var count = await videoQuery.CountAsync();
                var page = Convert.ToInt16(pageNum);
                if (keyEntity != null && count >= page * 25)
                {
                    var videoEntities = videoQuery
                        .AsNoTracking()
                        .OrderBy(v => v.SearchResultedVideoId)
                        .Skip((page - 1) * 25)
                        .Take(25)
                        .Include(v => v.Channel);

                    var videoDtos = videoEntities.MapVideoResultEntityToDto();
                    var entry = await videoEntities.FirstOrDefaultAsync();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoDtos, QueryNextPage = entry?.NextToken != null });
               
                }

                //API Call for fetching videos ids
                var lastEntity = await videoQuery.OrderByDescending(v => v.SearchResultedVideoId).FirstOrDefaultAsync();
                var nextToken = count > 0 ? lastEntity?.NextToken : null;
                var apiKey = configuration["YouTube:ApiKey"];
                var client = factory.CreateClient("YouTubeClient");
                var response = await client.GetAsync($"search?part=snippet&maxResults=25{(nextToken != null ? "&pageToken=" + nextToken : "")}&q={q}&key={apiKey}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of that search input are not found",
                        statusCode: 404
                    );
                }
                var json = await response.Content.ReadFromJsonAsync<SearchResultedVideos>();
                var emptyValue = Enumerable.Empty<VideoCardDto>().AsQueryable();
                if (json?.Items is null||json.Items.Length==0)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of that search input are not found",
                        statusCode: 404
                    );
                    //return TypedResults.Ok(new VideoCardListDto { VideoCardList = emptyValue, QueryNextPage = false });
                }
                var videosIds = string.Join("%2C",json.Items.Select(v => v.Id.VideoId));

                //API Call for fetching videos
                var response2 = await client.GetAsync($"videos?part=snippet%2CcontentDetails%2Cstatistics%2CliveStreamingDetails&id={videosIds}&key={apiKey}");
                if (!response2.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of those ids are not found",
                        statusCode: 404
                    );
                }
                var json2 = await response2.Content.ReadFromJsonAsync<VideosList>();
                if (json2?.Items == null || json2.Items.Length == 0)
                {
                    //return TypedResults.Ok(new VideoCardListDto { VideoCardList = emptyValue, QueryNextPage = false });
                    return Results.Problem(
                        title: "Not Found",
                        detail: "Videos of those ids are not found",
                        statusCode: 404
                    );
                }

                //update database
                if (keyEntity == null)
                {
                    await db.AddkeyToEntityAsync(q, json2.Etag);
                    keyEntity = await db.SearchKeys.SingleOrDefaultAsync(k => k.Title == q);
                }

                //VideoHelpers
                var dto = await videohelpers.VideosToDtosAsync(json2,keyEntity!,q, json?.NextPageToken);
                var data = new VideoCardListDto { VideoCardList = dto, QueryNextPage = json?.NextPageToken != null };
                return TypedResults.Ok(data);
            });
        }
    }
}
