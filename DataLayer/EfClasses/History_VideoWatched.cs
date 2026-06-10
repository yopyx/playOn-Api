using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    // Explicit Join Entity (ManyToMany Relationship)
    public class History_VideoWatched
    {
        public int History_VideoWatchedId { get; set; }
        public int HistoryId { get; set; }
        public History History { get; set; } = null!;
        public int VideoWatchedId {  get; set; }
        public VideoWatched VideoWatched { get; set; } = null!;
        public DateTime WatchedAt { get; set; } // or any Order index
    }
}
