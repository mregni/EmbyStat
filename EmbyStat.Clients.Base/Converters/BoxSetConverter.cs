using System.Linq;
using EmbyStat.Clients.Base.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Clients.Base.Converters
{
    public static class BoxSetConverter
    {
        public static BoxSet ConvertToBoxSet(this BaseItemDto x)
        {
            return new BoxSet
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                OfficialRating = x.OfficialRating,
                Primary = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value
            };
        }
    }
}
