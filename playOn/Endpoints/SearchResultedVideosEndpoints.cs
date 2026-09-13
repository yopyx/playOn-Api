using System.Collections.Generic;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
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
            app.MapGet("/search_results", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db, IVideoHelpers videohelpers, string q, string pageNum) =>
            {
                //Database Query
                q = q.Trim().ToLower();
                var page = Convert.ToInt16(pageNum);
                var keyEntity = await db.SearchKeys.FirstOrDefaultAsync(k => k.Title==q);
                //var videoQuery = db.VideoResults.Where(v => v.SearchKey.Title == q && v.PageNum == page);
                //var count = await videoQuery.CountAsync();

                if (keyEntity!=null && keyEntity.PageNum >= page)
                {
                    var videoEntities = db.VideoResults
                        .Where(v => v.SearchKey.Title == q && v.PageNum == page)
                        .AsNoTracking()
                        .OrderBy(v => v.SearchResultedVideoId)
                        .Include(v => v.Channel);

                    var videoDtos = videoEntities.MapVideoResultEntityToDto();
                    var entry = await videoEntities.FirstOrDefaultAsync();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoDtos, QueryNextPage = entry?.NextToken != null });
                }

                //API Call for fetching videos ids
                var lastEntity = await db.VideoResults.Where(v => v.SearchKey.Title == q).OrderByDescending(v => v.SearchResultedVideoId).FirstOrDefaultAsync();
                var nextToken = lastEntity?.NextToken;
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
                var videosIds = string.Join("%2C",json.Items.Where(v => v.Id != null && !string.IsNullOrEmpty(v.Id.VideoId)).DistinctBy(v => v.Id.VideoId).Select(v => v.Id.VideoId));

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
                try
                {
                    if (keyEntity == null)
                    {
                        await db.AddkeyToEntityAsync(q);
                    }
                    else
                    {
                        await db.UpdateKeyEntityAsync(keyEntity);
                    }
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    // Someone else already wrote this — not a real error, just detach and re-read.
                    foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                        entry.State = EntityState.Detached;
                }

                //VideoHelpers
                var updatedKeyEntity = await db.SearchKeys.FirstOrDefaultAsync(k => k.Title == q);
                var dto = await videohelpers.VideosToDtosAsync(json2, json?.NextPageToken,updatedKeyEntity);
                var data = new VideoCardListDto { VideoCardList = dto, QueryNextPage = json?.NextPageToken != null };
                return TypedResults.Ok(data);
            });
        }
    }
}
