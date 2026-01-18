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
    public class ChannelHelpers(EfCoreContext db,IHttpClientFactory factory, IConfiguration configuration) : IChannelHelpers //primary constructor syntax instead of defining the constructor
    {
        private readonly EfCoreContext _db = db;
        private readonly IHttpClientFactory _factory = factory;
        private readonly IConfiguration _configuration = configuration;

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

            //API Call
            var ids = String.Join("%2C", channelsIdsNotInDb);
            var apiKey = _configuration["YouTube:ApiKey"];
            var client = _factory.CreateClient("YouTubeClient");
            var response = await client.GetAsync($"channels?part=snippet%2Cstatistics&id={ids}&key={apiKey}");
            var emptyValue = Enumerable.Empty<Channel>().AsQueryable();
            if (!response.IsSuccessStatusCode)
            {
                //var value = new List<ChannelDto>().AsQueryable();
                //var value = Array.Empty<ChannelDto>().AsQueryable();
                return emptyValue;
                ; }
            var json = await response.Content.ReadFromJsonAsync<Channels>();
            if (json?.Items is null)
            {
                return emptyValue;
            }
        }
    }
}
