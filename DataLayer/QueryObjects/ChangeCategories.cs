using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using static Shared.DataTypes;

namespace DataLayer.QueryObjects
{
    public static class ChangeCategories
    {
        public static async Task AddCategoriesToEntity(this EfCoreContext db, List<VideoCategoryFromApi> categories) {
            var entities = categories.Select(category => new VideoCategory {
                VCId = category.Id,
                Etag = category.Etag,
                Title = category.Snippet.Title,
            });

            await db.VideoCategories.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }
        public static async Task UpdateCategoriesEntitiesAsync(this EfCoreContext db, VideoCategory category)
        {
            category.PageNum++;
            await db.SaveChangesAsync();
        }
    }
}
