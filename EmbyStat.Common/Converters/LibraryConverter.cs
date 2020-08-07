using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Common.Converters
{
    public static class LibraryConverter
    {
        public static Library ConvertToLibrary(BaseItemDto dto)
        {
            return new Library
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.CollectionType.ToLibraryType(),
                PrimaryImage = dto.ImageTags.ContainsKey(ImageType.Primary)
                    ? dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value
                    : default
            };
        }
    }
}
