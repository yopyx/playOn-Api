using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EfClasses
{
    public class User
    {
        public int UserId { get; set; }
        public string UId { get; set; } = string.Empty;

        //Relationships
        public ICollection<History> HistoryRecords { get; set; } = []; // navigation property
    }
}
