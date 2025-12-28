using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class VideoCategory
    {
        public int VideoCategoryId { get; set; }
        public string VCId { get; set; } = string.Empty;
        public string? Etag { get; set; }
        public string Title { get; set; } = string.Empty;


        //Relationships
        public ICollection<Video> Videos { get; set; } = [];
    }
}
