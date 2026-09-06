using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.QueryObjects
{
    public static class ChangeSearchKeys
    {
        public static async Task AddkeyToEntityAsync(this EfCoreContext db, string q)
        {
            var entity = new SearchKey
            {
                Title = q,
            };

            await db.SearchKeys.AddAsync(entity);
            await db.SaveChangesAsync();
        }
        public static async Task UpdateKeyEntityAsync(this EfCoreContext db, SearchKey key)
        {
            key.PageNum++;
            await db.SaveChangesAsync();
        }
    }
}
