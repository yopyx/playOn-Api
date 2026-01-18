using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;
using static Shared.Dtos;

namespace playOn.Helpers
{
    public interface IChannelHelpers
    {
        Task<IQueryable<Channel>> GetChannelsAsync(IEnumerable<string> channelsIds);
    }
    public class ChannelHelpers(EfCoreContext db) : IChannelHelpers //primary constructor syntax instead of defining the constructor
    {
        private readonly EfCoreContext _db = db;

        // returning channel entites
        public async Task<IQueryable<Channel>> GetChannelsAsync(IEnumerable<string> channelsIds) {

            //Database Query
            var idsInDbHash = _db.Channels.Select(c => c.ChId).ToHashSet();
            var channelsIdsNotInDb = channelsIds.Where(id => !idsInDbHash.Contains(id)).ToHashSet();
            if (channelsIdsNotInDb.Count==0)
            {
                var channelEntities = await _db.Channels.Where(c => channelsIds.Contains(c.ChId)).ToListAsync();
                return channelEntities.AsQueryable();
            }
        }
    }
}
