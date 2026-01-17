using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.VideoServices
{
    public class VideoCategoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Etag { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
