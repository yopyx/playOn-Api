using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.DataTypes;

namespace Shared
{
    public class Dtos
    {
        public class ChannelDto
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public Dictionary<string, ThumbnailDetails>? Thumbnails { get; set; } = [];
            public string SubscriberCount { get; set; } = string.Empty;
        }
    }
}