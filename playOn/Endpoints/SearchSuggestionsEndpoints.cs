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
using Npgsql;

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
                var entity = await db.SearchSuggestions.AsNoTracking().FirstOrDefaultAsync(s => s.Title==q);
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
                if (list != null && list.Length > 0)
                {
                    try {
                        await db.AddSuggestionsToEntityAsync(q, list!);
                    }
                    // unique constraint violation
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                    {
                        // Someone else already wrote this — not a real error, just detach and re-read.
                        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                            entry.State = EntityState.Detached;
                    }
                }

                return TypedResults.Ok(list ?? []);
            });
        }
    }
}
