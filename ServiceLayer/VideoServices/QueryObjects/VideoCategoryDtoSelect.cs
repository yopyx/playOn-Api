using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using static Shared.DataTypes;

namespace ServiceLayer.VideoServices.QueryObjects
{
    public static class VideoCategoryDtoSelect
    {
        public static IQueryable<VideoCategoryDto> MapCategoryEntityToDto(this IQueryable<VideoCategory> videoCategories) {
            return videoCategories.Select(category => new VideoCategoryDto
            {
                Id = category.VCId,
                Etag = category.Etag,
                Title = category.Title,
            });
        }

        public static IQueryable<VideoCategoryDto> MapCategoryToDto(this IQueryable<VideoCategoryFromApi> videoCategories)
        {
            return videoCategories.Select(category => new VideoCategoryDto
            {
                Id = category.Id,
                Etag = category.Etag,
                Title = category.Snippet.Title,
            });
        }
    }
}
