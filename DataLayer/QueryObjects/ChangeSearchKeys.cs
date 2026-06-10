using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace DataLayer.QueryObjects
{
    public static class ChangeSearchKeys
    {
        public static async Task AddkeyToEntityAsync(this EfCoreContext db, string q, string etag)
        {
            var entity = new SearchKey
            {
                Title = q,
                VideoListEtag = etag
            };

            await db.SearchKeys.AddAsync(entity);
            await db.SaveChangesAsync();
        }
    }
}
