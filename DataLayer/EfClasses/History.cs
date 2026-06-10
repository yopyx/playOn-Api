using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.DataTypes;

namespace DataLayer.EfClasses
{
    public class History
    {
        public int HistoryId { get; set; }
        public DateOnly Date {  get; set; }

        //Relationships
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<History_VideoWatched> VideoWatchedLinks { get; set; } = []; // navigation property
    }
}
