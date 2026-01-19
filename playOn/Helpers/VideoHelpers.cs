using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using playOn.Contracts;
using ServiceLayer.VideoServices;
using DataLayer.QueryObjects;
using ServiceLayer.VideoServices.QueryObjects;

namespace playOn.Helpers
{
    public interface IVideoHelpers {
        Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, SearchKey? key,string q);
    }
    public class VideoHelpers(EfCoreContext db, IChannelHelpers helpers) : IVideoHelpers
    {
        private readonly EfCoreContext _db = db;
        private readonly IChannelHelpers _helpers = helpers;
        public async Task<IQueryable<VideoCardDto>> VideosToDtosAsync(VideosList list, SearchKey? key, string q){

            //Extracting channels
            var channelIds = list.Items.Select(v => v.Snippet.ChannelId).Distinct();
            var channels = await _helpers.GetChannelsAsync(channelIds);

            //Update Database
            var searchKeyEntity = key ?? new SearchKey
            {
                Title = q,
                NextToken = list?.NextPageToken,
                VideoListEtag = list!.Etag
            };
            await _db.AddResultsToEntityAsync(list.Items, channels, searchKeyEntity);

            var videoCardsToDto = channels.MapVideoCardToDto(list.Items, list.Etag);
            return videoCardsToDto;
        }
    }
}
