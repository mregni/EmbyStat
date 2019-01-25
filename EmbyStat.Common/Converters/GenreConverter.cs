using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Common.Converters
{
    public static class GenreConverter
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
