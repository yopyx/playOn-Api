using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace playOn.Endpoints
{
    public static class SearchSuggestionsEndpoints
    {
        public static void MapSearchSuggestionsEndpoints(this RouteGroupBuilder app)
        {
            app.MapGet("/search_suggestions", [Authorize(policy: "ApiUser")] async (EfCoreContext db, string q) =>
            {

                //Database Query
                q = q.Trim().ToLower();
                var entity = await db.SearchSuggestions.AsNoTracking().SingleOrDefaultAsync(s => s.Title==q);
                if (entity != null)
                {
                    return TypedResults.Ok(entity.Suggestions);
                }

                //API Call
                var client = new HttpClient();
                if (q.Trim() == string.Empty)
                {
                    return Results.ValidationProblem(new Dictionary<string, string[]>{
                        {"q", new[] {"A non nullified search query input must be provided"}}
                    });
                }
                var response = await client.GetAsync($@"https://suggestqueries.google.com/complete/search?client=firefox&ds=yt&q={q}");
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(
                        title: "Not Found",
                        detail: "The search suggestions are not found",
                        statusCode: 404
                    );
                }
                JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
                var list = json[1].EnumerateArray().Select(x => x.GetString()).ToArray();
                //Database Update
                if (list!=null && list.Length > 0) { 
                    await db.AddSuggestionsToEntityAsync(q, list!);
                }

                return TypedResults.Ok(list ?? []);
            });
        }
    }
}
