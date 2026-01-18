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
            });
        }
    }
}
