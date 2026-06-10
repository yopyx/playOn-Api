using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace DataLayer.QueryObjects
{
    public static class ChangeUsers_HistoryRecords
    {
        public static async Task AddWatchedEntityToUserHistoryAsync(this EfCoreContext db, User? user, VideoWatched video, string sub) {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var historyEntity = user?.HistoryRecords.FirstOrDefault(h => h.Date == today);
            
            if (user == null)
            {
                user = new User { UId = sub };
                db.Users.Add(user);
            }

            if (historyEntity == null)
            {
                historyEntity = new History { Date = today };
                user.HistoryRecords.Add(historyEntity);
            }

            var entity = new History_VideoWatched
            {
                VideoWatched = video,
                WatchedAt = DateTime.UtcNow,
            };

            if (historyEntity.VideoWatchedLinks.Count==0 || historyEntity.VideoWatchedLinks.Last().VideoWatchedId!=video.VideoWatchedId)
            {
                historyEntity.VideoWatchedLinks.Add(entity);
            }
            await db.SaveChangesAsync();
        }
    }
}
