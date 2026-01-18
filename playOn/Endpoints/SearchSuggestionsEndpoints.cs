using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace playOn.Endpoints
{
    public static class SearchSuggestionsEndpoints
    {
        public static void MapSearchSuggestionsEndpoints(this WebApplication app)
        {
            app.MapGet("/search_suggestions", async (EfCoreContext db, string q) =>
            {

                //Database Query
                q = q.Trim().ToLower();
                var entity = await db.SearchSuggestions.SingleOrDefaultAsync(s => s.Title==q);
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
                    return Results.Problem($"API call failed: {response.StatusCode}");
                }
                JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
                var list = json[1].EnumerateArray().Select(x => x.GetString()).ToArray();
            });
        }
    }
}
