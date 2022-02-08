using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using Newtonsoft.Json;
using SqlPerson = EmbyStat.Common.SqLite.SqlPerson;

namespace EmbyStat.Clients.Base.Converters
{
    public static class PersonConverter
    {
        public static SqlPerson ConvertToPeople(this BaseItemDto dto, Logger logger)
        {
            try
            {
                return new SqlPerson
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Primary = dto.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                    BirthDate = dto.PremiereDate
                };
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Person");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }
    }
}
