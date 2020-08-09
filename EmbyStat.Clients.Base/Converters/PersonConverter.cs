using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Clients.Base.Converters
{
    public static class PersonConverter
    {
        public static Person Convert(BaseItemDto dto, Logger logger)
        {
            try
            {
                return new Person
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Primary = dto.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                    BirthDate = dto.PremiereDate,
                    Etag = dto.Etag,
                    IMDB = dto.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value,
                    TMDB = dto.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value,
                    OverView = dto.Overview,
                    SortName = dto.SortName
                };
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("DTO object tried to convert");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }
    }
}
