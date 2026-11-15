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
        public static async Task<SearchKey> AddkeyToEntityAsync(this EfCoreContext db, string q)
        {
            var entity = new SearchKey
            {
                Title = q,
                PageNum = 1
            };

            await db.SearchKeys.AddAsync(entity);
            return entity;
        }
        public static void UpdateKeyEntityAsync(SearchKey key, int page)
        {
            if (key.PageNum < page) {
                key.PageNum++;
            }
        }
    }
}
