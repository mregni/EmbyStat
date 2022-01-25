using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Clients.Base.Converters
{
    public static class GenreConverter
    {
        public static SqlGenre ConvertToGenre(this BaseItemDto dto, Logger logger)
        {
            try
            {
                return new SqlGenre
                {
                    Id = dto.Id,
                    Name = dto.Name
                };
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Genre");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }
    }
}
