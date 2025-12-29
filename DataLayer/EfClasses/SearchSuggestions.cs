using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class SearchSuggestions
    {
        public int SearchSuggestionsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string[] Suggestions { get; set; } = [];
    }
}
