using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Helpers
{
    public interface IVideoHelpers {
        Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, string? token, string q, int page);
    }
    public class VideoHelpers(EfCoreContext db, IChannelHelpers helpers) : IVideoHelpers
    {
        private readonly EfCoreContext _db = db;
        private readonly IChannelHelpers _helpers = helpers;
        public async Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, string? token, string q, int page){

            //Extracting channels
            var channelIds = list.Items.Select(v => v.Snippet.ChannelId).Distinct();
            var channels = await _helpers.GetChannelsAsync(channelIds);
            var keyEntity = await _db.SearchKeys.FirstOrDefaultAsync(k => k.Title == q);

            //Update Database
            if (list.Items.Length > 0) {
                try { 
                    await _db.AddResultsToEntityAsync(list.Items, channels, keyEntity, list.Etag, token,keyEntity?.PageNum ?? page); 
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
                {
                    // Someone else already wrote this — not a real error, just detach and re-read.
                    foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                        entry.State = EntityState.Detached;
                }
            }

            var videoCardsToDto = channels.MapVideoCardToDto(_db,list!.Items, list.Etag);
            return videoCardsToDto;
        }
    }
}
