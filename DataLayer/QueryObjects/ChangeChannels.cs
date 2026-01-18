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
    public static class ChangeChannels
    {
        public static async Task AddChannelsToEntity(this EfCoreContext db,ChannelFromApi[] channels) {
            var entities = channels.Select(c => new Channel {
                ChId = c.Id,
                Title = c.Snippet.Title,
                Thumbnails = c.Snippet.Thumbnails,
                Statistics = c.Statistics,
            });
            await db.Channels.AddRangeAsync(entities);
            await db.SaveChangesAsync();
        }
    }
}
