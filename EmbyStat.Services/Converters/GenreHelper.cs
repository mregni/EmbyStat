using EmbyStat.Common.Models;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Services.Converters
{
    public static class GenreHelper
    {
        public static Genre ConvertToGenre(BaseItemDto genre)
        {
            return new Genre
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
    }
}
