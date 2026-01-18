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

                //API Call
                var client = factory.CreateClient("YouTubeClient");
                string apiKey = configuration["YouTube:ApiKey"]!;
                var response = await client.GetAsync($"videoCategories?part=snippet&regionCode=US&key={apiKey}");

                if (!response.IsSuccessStatusCode) { //API Call Fail Check
                    return Results.Problem($"API call failed: {response.StatusCode}");
                }

                var json = await response.Content.ReadFromJsonAsync<VideoCategories>();
                if(json?.Items == null||json.Items.Count==0)
                {
                    return TypedResults.Ok(Enumerable.Empty<VideoCategoryDto>().AsQueryable());
                }

                var filteredItems = json!.Items.Where(item => item.Snippet.Assignable == true).ToList();

                //Database Update
                await db.AddCategoriesToEntity(filteredItems);

                //Mapping filteredItems to Dto
                var categories = db.VideoCategories.MapCategoryEntityToDto();

                return TypedResults.Ok(categories);
            });
        }
    }
}
