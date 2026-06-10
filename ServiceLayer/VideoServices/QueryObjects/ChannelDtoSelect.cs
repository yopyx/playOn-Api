using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using static Shared.DataTypes;
using static Shared.Dtos;


namespace ServiceLayer.VideoServices.QueryObjects
{
    public static class ChannelDtoSelect
    {
        public static IQueryable<ChannelDto> MapChannelEntityToDto(this IQueryable<Channel> channels)
        {
            return channels.Select(c => new ChannelDto { 
                Id = c.ChId,
                Title = c.Title,
                Thumbnails = c.Thumbnails,
                SubscriberCount = c.Statistics!.SubscriberCount
            });
        }
        public static IQueryable<ChannelDto> MapChannelToDto(this IQueryable<ChannelFromApi> channels) {
            return channels.Select(c => new ChannelDto
            {
                Id = c.Id,
                Title = c.Snippet.Title,
                Thumbnails = c.Snippet.Thumbnails,
                SubscriberCount = c.Statistics!.SubscriberCount
            });
        }
    }
}
