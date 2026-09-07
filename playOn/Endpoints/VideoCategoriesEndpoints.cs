using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;
using static Shared.DataTypes;

namespace playOn.Endpoints
{
    public static class VideoCategoriesEndpoints
    {
        public static void MapVideoCategoriesEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/video_categories", [Authorize(policy: "ApiUser")] async (IHttpClientFactory factory, IConfiguration configuration, EfCoreContext db) => {
                
                //Database Query
                var count = await db.VideoCategories.CountAsync();
                if (count > 1)
                {
                    return TypedResults.Ok(db.VideoCategories.OrderBy(vc => vc.VideoCategoryId).AsNoTracking().MapCategoryEntityToDto());
                }

                //API Call
                var client = factory.CreateClient("YouTubeClient");
                string apiKey = configuration["YouTube:ApiKey"]!;
                var response = await client.GetAsync($"videoCategories?part=snippet&regionCode=US&key={apiKey}");

                if (!response.IsSuccessStatusCode) { //API Call Fail Check
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The categories are not found",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }

                var json = await response.Content.ReadFromJsonAsync<VideoCategories>();
                if(json?.Items == null||json.Items.Count==0)
                {
                    return TypedResults.Ok(Enumerable.Empty<VideoCategoryDto>().AsQueryable());
                }

                var filteredItems = json!.Items.Where(item => item.Snippet.Assignable == true).ToList();

                //Database Update
                try
                {
                    await db.AddCategoriesToEntity(filteredItems);
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    // Someone else already wrote this — not a real error, just detach and re-read.
                    foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                        entry.State = EntityState.Detached;
                }

                //Mapping filteredItems to Dto
                var categories = db.VideoCategories.MapCategoryEntityToDto();

                return TypedResults.Ok(categories);
            });
        }
    }
}
