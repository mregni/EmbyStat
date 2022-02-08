using System;
using System.Collections.Generic;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Clients.Base.Converters
{
    public static class MovieConverter
    {
        public static SqlMovie ConvertToMovie(this BaseItemDto dto, string collectionId, List<SqlGenre> genres, Logger logger)
        {
            try
            {
                var movie = new SqlMovie
                {
                    Id = dto.Id,
                    CollectionId = collectionId,
                    Name = dto.Name,
                    OriginalTitle = dto.OriginalTitle,
                    DateCreated = dto.DateCreated,
                    Path = dto.Path,
                    SortName = dto.SortName,
                    RunTimeTicks = dto.RunTimeTicks,
                    Container = dto.Container,
                    CommunityRating = dto.CommunityRating,
                    OfficialRating = dto.OfficialRating,
                    PremiereDate = dto.PremiereDate,
                    ProductionYear = dto.ProductionYear,
                    Video3DFormat = dto.Video3DFormat ?? 0
                };

                dto.MapImageTags(movie);
                dto.MapProviderIds(movie);
                dto.MapStreams(movie);
                dto.MapMediaSources(movie);
                dto.MapPeople(movie);

                return movie;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Movie");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }
    }
}
