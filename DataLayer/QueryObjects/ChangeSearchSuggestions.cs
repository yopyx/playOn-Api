using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace DataLayer.QueryObjects
{
    public static class ChangeSearchSuggestions
    {
        public static async Task AddSuggestionsToEntityAsync(this EfCoreContext db, string q, string[] suggestions) {
            var entity = new SearchSuggestions { 
                Title = q,
                Suggestions = suggestions
            };

            await db.SearchSuggestions.AddAsync(entity);
            await db.SaveChangesAsync();
        }
    }
}
