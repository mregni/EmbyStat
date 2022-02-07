using System;
using System.Collections.Generic;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Logging;
using Newtonsoft.Json;
using LocationType = EmbyStat.Common.Enums.LocationType;

namespace EmbyStat.Clients.Base.Converters
{
    public static class ShowConverter
    {
        public static Show ConvertToShow(this BaseItemDto dto, string libraryId, Logger logger)
        {
            try
            {
                var show = new Show
                {
                    Id = dto.Id,
                    CollectionId = libraryId,
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    Path = dto.Path,
                    CommunityRating = dto.CommunityRating,
                    DateCreated = dto.DateCreated,
                    OfficialRating = dto.OfficialRating,
                    PremiereDate = dto.PremiereDate,
                    ProductionYear = dto.ProductionYear,
                    RunTimeTicks = dto.RunTimeTicks,
                    SortName = dto.SortName,
                    Status = dto.Status,
                    Genres = dto.Genres,
                    Seasons = new List<Season>(),
                    Episodes = new List<Episode>()
                };

                //dto.MapPeople(show);
                //dto.MapImageTags(show);
                //dto.MapProviderIds(show);

                return show;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Show");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }

        public static Season ConvertToSeason(this BaseItemDto dto, Logger logger)
        {
            try
            {
                var season = new Season
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    Path = dto.Path,
                    DateCreated = dto.DateCreated,
                    IndexNumber = dto.IndexNumber,
                    IndexNumberEnd = dto.IndexNumberEnd,
                    PremiereDate = dto.PremiereDate,
                    ProductionYear = dto.ProductionYear,
                    SortName = dto.SortName,
                    LocationType = LocationType.Disk
                };

                //dto.MapImageTags(season);

                return season;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Season");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }

        public static SqlSeason ConvertToVirtualSeason(this int indexNumber, SqlShow show)
        {
            return new SqlSeason
            {
                Id = Guid.NewGuid().ToString(),
                Name = indexNumber == 0 ? "Special" : $"Season {indexNumber}",
                ShowId = show.Id,
                Path = string.Empty,
                DateCreated = null,
                IndexNumber = indexNumber,
                IndexNumberEnd = indexNumber,
                PremiereDate = null,
                ProductionYear = null,
                SortName = indexNumber.ToString("0000"),
                LocationType = LocationType.Virtual,
                Episodes = new List<SqlEpisode>()
            };
        }

        public static Episode ConvertToEpisode(this BaseItemDto dto, string showId, Logger logger)
        {
            try
            {
                var episode = new Episode
                {
                    Id = Guid.NewGuid().ToString(),
                    ShowId = showId,
                    LocationType = LocationType.Disk,
                    Name = dto.Name,
                    Path = dto.Path,
                    ParentId = dto.ParentId,
                    CommunityRating = dto.CommunityRating,
                    Container = dto.Container,
                    DateCreated = dto.DateCreated,
                    IndexNumber = dto.IndexNumber,
                    IndexNumberEnd = dto.IndexNumberEnd,
                    MediaType = dto.Type,
                    ProductionYear = dto.ProductionYear,
                    PremiereDate = dto.PremiereDate,
                    RunTimeTicks = dto.RunTimeTicks,
                    SortName = dto.SortName,
                    ShowName = dto.SeriesName
                };

                //dto.MapImageTags(episode);
                //dto.MapProviderIds(episode);
                //dto.MapStreams(episode);
                //dto.MapMediaSources(episode);

                return episode;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.Debug("Tried to convert Episode");
                logger.Debug(JsonConvert.SerializeObject(dto));
                return null;
            }
        }

        public static SqlEpisode ConvertToVirtualEpisode(this VirtualEpisode episode, SqlSeason season)
        {
            return new SqlEpisode
            {
                Id = Guid.NewGuid().ToString(),
                Name = episode.Name,
                LocationType = LocationType.Virtual,
                IndexNumber = episode.EpisodeNumber,
                SeasonId = season.Id,
                PremiereDate = episode.FirstAired ?? new DateTime()
            };
        }
    }
}
