using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Clients.Base.Converters
{
    public static class BoxSetConverter
    {
        public static BoxSet ConvertToBoxSet(this BaseItemDto dto, Logger logger)
        {
            try
            {
                return new BoxSet
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    OfficialRating = dto.OfficialRating,
                    Primary = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value
                };
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert BoxSet");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }
    }
}
