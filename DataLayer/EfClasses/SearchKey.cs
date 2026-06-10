using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class SearchKey
    {
        public int SearchKeyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoListEtag { get; set; } = string.Empty;

        //Relationships

        public ICollection<SearchResultedVideo> Videos { get; set; } = [];
    }
}