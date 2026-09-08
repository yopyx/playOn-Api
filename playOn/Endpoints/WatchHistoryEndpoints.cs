using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using playOn.Helpers;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Endpoints
{
    public static class WatchHistoryEndpoints
    {
        public static void MapWatchHistoryEndpoints(this RouteGroupBuilder app) {
            app.MapGet("/feed_history", [Authorize(policy: "ApiUser")] async (IConfiguration configuration, EfCoreContext db, IChannelHelpers helpers, HttpContext http) => {

                //Extracting sub from claims principle
                var sub = http.User.FindFirst("sub")?.Value;

                var user = await db.Users.FirstOrDefaultAsync(u => u.UId==sub);
                var records = await db.Set<History>().AsNoTracking()
                    .Where(h => h.User.UId == sub)
                    .OrderByDescending(h => h.HistoryId)
                    .Select(h => new
                    {
                        h.Date,
                        Videos = h.VideoWatchedLinks
                            .OrderByDescending(hv => hv.WatchedAt)
                            .Select(hv => hv.VideoWatched.MapOneWatchedEntityToDto(hv.VideoWatched.Channel))
                    })
                    .ToDictionaryAsync(x => x.Date, x => x.Videos);

                if (user != null) {
                    return TypedResults.Ok(records);
                }

                return TypedResults.Ok<Dictionary<DateOnly, IEnumerable<VideoCardDto>>>([]);
            });
        }
    }
}
