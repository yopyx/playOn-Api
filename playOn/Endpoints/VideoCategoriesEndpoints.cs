using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;
using static Shared.DataTypes;

namespace playOn.Endpoints
{
    public static class VideoCategoriesEndpoints
    {
        public static void MapVideoCategoriesEndpoints(this WebApplication app)
        {
            app.MapGet("/video_categories", async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db) => {
                
                //Database Query
                var count = await db.VideoCategories.CountAsync();
                if (count > 1)
                {
                    return TypedResults.Ok(db.VideoCategories.MapCategoryEntityToDto());
                }
            });
        }
    }
}
