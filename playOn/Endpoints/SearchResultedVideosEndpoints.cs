using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using ServiceLayer.SearchResultServices.QueryObjects;
using ServiceLayer.VideoServices;
using Microsoft.AspNetCore.Mvc;


namespace playOn.Endpoints
{
    public static class SearchResultedVideosEndpoints
    {
        public static void MapSearchResultedVideosEndpoints(this WebApplication app)
        {
            app.MapGet("/search_results", async (IHttpClientFactory factory, IConfiguration configuration, [FromServices] EfCoreContext db, string q, string pageNum) =>
            {
                //Database Query
                q = q.Trim().ToLower();
                var videoQuery = db.VideoResults.Where(v => v.SearchKey.Title == q);
                var count = await videoQuery.CountAsync();
                //var entity = await db.SearchKeys.SingleOrDefaultAsync(k => k.Title == q);
                var nextToken = await videoQuery.OrderByDescending(v => v.SearchResultedVideoId).Select(v => v.NextToken).FirstOrDefaultAsync();
                if (count >= Convert.ToInt16(pageNum) * 25)
                {
                    var videoEntities = videoQuery
                        .OrderBy(v => v.SearchResultedVideoId)
                        .Skip((Convert.ToInt16(pageNum) - 1) * 25)
                        .Take(25)
                        .MapVideoResultEntityToDto();
                    return TypedResults.Ok(new VideoCardListDto { VideoCardList = videoEntities, QueryNextPage = nextToken != null });
                }
            });
        }
    }
}
