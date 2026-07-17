using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Helpers
{
    public interface IVideoHelpers {
        Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, SearchKey key,string q, string? token);
    }
    public class VideoHelpers(EfCoreContext db, IChannelHelpers helpers) : IVideoHelpers
    {
        private readonly EfCoreContext _db = db;
        private readonly IChannelHelpers _helpers = helpers;
        public async Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, SearchKey key, string q, string? token){

            //Extracting channels
            var channelIds = list.Items.Select(v => v.Snippet.ChannelId).Distinct();
            var channels = await _helpers.GetChannelsAsync(channelIds);

            //Update Database
            if (list.Items.Length > 0) {
                try { 
                    await _db.AddResultsToEntityAsync(list.Items, channels, key, list.Etag, token); 
                }
                catch (DbUpdateException)
                {
                    _db.ChangeTracker.Clear();
                }
            }

            var videoCardsToDto = channels.MapVideoCardToDto(_db,list!.Items, list.Etag);
            return videoCardsToDto;
        }
    }
}
