using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.Models.Show;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using Newtonsoft.Json;
using LocationType = EmbyStat.Common.Enums.LocationType;

namespace EmbyStat.Services
{
    public class ShowService : MediaService, IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ISettingsService _settingsService;

        public ShowService(IJobRepository jobRepository, IShowRepository showRepository, ILibraryRepository libraryRepository,
            IPersonService personService, IStatisticsRepository statisticsRepository, ISettingsService settingsService) : base(jobRepository, personService)
        {
            _showRepository = showRepository;
            _libraryRepository = libraryRepository;
            _statisticsRepository = statisticsRepository;
            _settingsService = settingsService;
        }

        public IEnumerable<Library> GetShowLibraries()
        {
            var settings = _settingsService.GetUserSettings();
            return _libraryRepository.GetLibrariesById(settings.ShowLibraries);
        }

        public ShowStatistics GetStatistics(List<string> libraryIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Show, libraryIds);

            if (StatisticsAreValid(statistic, libraryIds, Constants.JobIds.ShowSyncId))
            {
                return JsonConvert.DeserializeObject<ShowStatistics>(statistic.JsonResult);
            }

            return CalculateShowStatistics(libraryIds);
        }

        public ShowStatistics CalculateShowStatistics(string libraryId)
        {
            return CalculateShowStatistics(new List<string> { libraryId });
        }

        public ShowStatistics CalculateShowStatistics(List<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);

            var statistics = new ShowStatistics
            {
                Cards = CalculateCards(libraryIds),
                TopCards = CalculateTopCards(libraryIds),
                People = CalculatePeopleStatistics(libraryIds),
                BarCharts = CalculateBarCharts(shows),
                PieCharts = CalculatePieChars(shows)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Show, libraryIds);

            return statistics;
        }

        public bool TypeIsPresent()
        {
            return _showRepository.Any();
        }

        public Page<ShowRow> GetShowPage(int skip, int take, string sort, Filter[] filters, bool requireTotalCount, List<string> libraryIds)
        {
            var list = _showRepository
                .GetShowPage(skip, take, sort, filters, libraryIds)
                .Select(x => new ShowRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    SortName = x.SortName,
                    CollectedEpisodeCount = x.GetEpisodeCount(false, LocationType.Disk),
                    MissingEpisodesCount = x.GetEpisodeCount(false, LocationType.Virtual),
                    SpecialEpisodeCount = x.GetEpisodeCount(true, LocationType.Disk),
                    Genres = x.Genres,
                    OfficialRating = x.OfficialRating,
                    CumulativeRunTimeTicks = x.CumulativeRunTimeTicks,
                    RunTime = x.RunTimeTicks,
                    Status = x.Status
                });

            var page = new Page<ShowRow> { Data = list };
            if (requireTotalCount)
            {
                page.TotalCount = _showRepository.GetMediaCount(filters, libraryIds);
            }

            return page;
        }

        public Show GetShow(string id)
        {
            return _showRepository.GetShowById(id, true);
        }

        #region Cards

        private List<Card<string>> CalculateCards(IReadOnlyList<string> libraryIds)
        {
            return new List<Card<string>>
            {
                CalculateTotalShowCount(libraryIds),
                CalculateTotalEpisodeCount(libraryIds),
                CalculateTotalMissingEpisodeCount(libraryIds),
                CalculatePlayableTime(libraryIds),
                CalculateTotalShowGenres(libraryIds),
                CalculateTotalDiskSize(libraryIds)
            };
        }

        private Card<string> CalculateTotalShowCount(IReadOnlyList<string> libraryIds)
        {
            var count = _showRepository
                .GetMediaCount(libraryIds)
                .ToString();

            return new Card<string>
            {
                Title = Constants.Shows.TotalShows,
                Value = count,
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }

        private Card<string> CalculateTotalEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            var sum = _showRepository
                .GetAllShows(libraryIds, true, true)
                .Sum(x => x.GetEpisodeCount(false, LocationType.Disk))
                .ToString();

            return new Card<string>
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = sum,
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }

        private Card<string> CalculateTotalShowGenres(IReadOnlyList<string> libraryIds)
        {
            var totalGenres = _showRepository.GetGenreCount(libraryIds);
            return new Card<string>
            {
                Title = Constants.Common.TotalGenres,
                Value = totalGenres.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }

        private Card<string> CalculateTotalMissingEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            var sum = _showRepository
                .GetAllShows(libraryIds, false, true)
                .Sum(x => x.GetEpisodeCount(false, LocationType.Virtual))
                .ToString();

            var boe = _showRepository.GetAllShows(libraryIds, false, true).ToList();
            var epi = boe.SelectMany(x => x.Episodes);

            var epiw = epi.Where(x => x.LocationType == LocationType.Virtual).ToList();

            return new Card<string>
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = sum,
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }

        private Card<string> CalculatePlayableTime(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, false, false);
            var playLength = new TimeSpan(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));

            return new Card<string>
            {
                Title = Constants.Shows.TotalPlayLength,
                Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                Type = CardType.Time,
                Icon = Constants.Icons.QueryBuilderRoundedIcon
            };
        }

        private Card<string> CalculateTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            var sum = _showRepository
                .GetAllShows(libraryIds, false, true)
                .SelectMany(x => x.Episodes)
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0);

            return new Card<string>
            {
                Value = sum.ToString(CultureInfo.InvariantCulture),
                Title = Constants.Common.TotalDiskSize,
                Type = CardType.Size,
                Icon = Constants.Icons.StorageRoundedIcon
            };
        }

        #endregion

        #region TopCards

        private List<TopCard> CalculateTopCards(IReadOnlyList<string> libraryIds)
        {
            var list = new List<TopCard>();

            var calculateNewestPremieredShow = CalculateNewestPremieredShow(libraryIds);
            list.AddIfNotNull(calculateNewestPremieredShow);

            var calculateOldestPremieredShow = CalculateOldestPremieredShow(libraryIds);
            list.AddIfNotNull(calculateOldestPremieredShow);

            var calculateLatestAddedShow = CalculateLatestAddedShow(libraryIds);
            list.AddIfNotNull(calculateLatestAddedShow);

            var calculateHighestRatedShow = CalculateHighestRatedShow(libraryIds);
            list.AddIfNotNull(calculateHighestRatedShow);

            var calculateLowestRatedShow = CalculateLowestRatedShow(libraryIds);
            list.AddIfNotNull(calculateLowestRatedShow);

            var calculateShowWithMostEpisodes = CalculateShowWithMostEpisodes(libraryIds);
            list.AddIfNotNull(calculateShowWithMostEpisodes);

            return list;
        }

        private TopCard CalculateNewestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetNewestPremieredMedia(libraryIds, 5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                : null;
        }

        private TopCard CalculateOldestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetOldestPremieredMedia(libraryIds, 5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                : null;
        }

        private TopCard CalculateLatestAddedShow(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetLatestAddedMedia(libraryIds, 5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.LatestAdded, "COMMON.DATE", "DateCreated", ValueTypeEnum.Date)
                : null;
        }

        private TopCard CalculateHighestRatedShow(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetHighestRatedMedia(libraryIds, 5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.HighestRatedShow, "/10", "CommunityRating", false)
                : null;
        }

        private TopCard CalculateLowestRatedShow(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetLowestRatedMedia(libraryIds, 5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.LowestRatedShow, "/10", "CommunityRating", false)
                : null;
        }

        private TopCard CalculateShowWithMostEpisodes(IReadOnlyList<string> libraryIds)
        {
            var list = _showRepository.GetShowsWithMostEpisodes(libraryIds, 5);

            return list.Count > 0
                ? list.ConvertToTopCard(Constants.Shows.MostEpisodes, "#")
                : null;
        }

        #endregion

        #region Charts

        private List<Chart> CalculateBarCharts(IReadOnlyList<Show> shows)
        {
            return new List<Chart>
            {
                CalculateGenreChart(shows),
                CalculateRatingChart(shows.Select(x => x.CommunityRating)),
                CalculatePremiereYearChart(shows.Select(x => x.PremiereDate)),
                CalculateCollectedRateChart(shows)
            };
        }

        private List<Chart> CalculatePieChars(IReadOnlyList<Show> shows)
        {
            return new List<Chart>
            {
                CalculateOfficialRatingChart(shows),
                CalculateShowStateChart(shows)
            };
        }

        private Chart CalculateShowStateChart(IReadOnlyList<Show> shows)
        {
            var list = shows
                .GroupBy(x => x.Status)
                .Select(x => new { Label = x.Key, Val0 = x.Count() })
                .OrderByDescending(x => x.Val0)
                .ToList();

            return new Chart
            {
                Title = Constants.Shows.ShowStatusChart,
                DataSets = JsonConvert.SerializeObject(list),
                SeriesCount = 1

            };
        }

        private Chart CalculateOfficialRatingChart(IReadOnlyList<Show> shows)
        {
            var ratingData = shows
                .Where(x => !string.IsNullOrWhiteSpace(x.OfficialRating))
                .GroupBy(x => x.OfficialRating.ToUpper())
                .Select(x => new { Label = x.Key, Val0 = x.Count() })
                .OrderByDescending(x => x.Val0)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerOfficialRating,
                DataSets = JsonConvert.SerializeObject(ratingData),
                SeriesCount = 1
            };
        }

        private Chart CalculateCollectedRateChart(IReadOnlyList<Show> shows)
        {
            var percentageList = new List<double>();
            foreach (var show in shows)
            {
                var specialSeasonId = show.Seasons.FirstOrDefault(x => x.IndexNumber == 0)?.Id.ToString() ?? "0";
                var episodeCount = show.Episodes.Count(x => x.LocationType == LocationType.Disk && x.ParentId != specialSeasonId);
                var missingEpisodeCount = show.Episodes.Count(x => x.LocationType == LocationType.Virtual && x.ParentId != specialSeasonId);

                if (episodeCount + missingEpisodeCount == 0)
                {
                    percentageList.Add(0);
                }
                else
                {
                    percentageList.Add((double)episodeCount / (episodeCount + missingEpisodeCount));
                }
            }

            var groupedList = percentageList
                .GroupBy(x => x.RoundToFive())
                .ToList();

            for (var i = 0; i < 20; i++)
            {
                if (groupedList.All(x => x.Key != i * 5))
                {
                    groupedList.Add(new ChartGrouping<int?, double> { Key = i * 5, Capacity = 0 });
                }
            }

            var rates = groupedList
                .Where(x => x.Key != 100)
                .OrderBy(x => x.Key)
                .Select(x => new { Label = x.Key != 100 ? $"{x.Key}% - {x.Key + 4}%" : $"{x.Key}%", Val0 = x.Count() })
                .Select(x => new { x.Label, x.Val0 })
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerCollectedPercentage,
                DataSets = JsonConvert.SerializeObject(rates),
                SeriesCount = 1
            };
        }

        #endregion

        #region People

        public PersonStats CalculatePeopleStatistics(IReadOnlyList<string> libraryIds)
        {
            var returnObj = new PersonStats();
            try
            {
                returnObj.Cards = new List<Card<string>>
                {
                    TotalTypeCount(libraryIds, PersonType.Actor, Constants.Common.TotalActors)
                };

                return returnObj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private Card<string> TotalTypeCount(IReadOnlyList<string> libraryIds, PersonType type, string title)
        {
            var value = _showRepository.GetPeopleCount(libraryIds, type);
            return new Card<string>
            {
                Value = value.ToString(),
                Title = title,
                Icon = Constants.Icons.PeopleAltRoundedIcon,
                Type = CardType.Text
            };
        }

        #endregion

        #region Collected Rows

        public ListContainer<ShowCollectionRow> GetCollectedRows(List<string> libraryIds, int page)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCollectedRows, libraryIds);

            ListContainer<ShowCollectionRow> rows = new ListContainer<ShowCollectionRow>();
            if (StatisticsAreValid(statistic, libraryIds, Constants.JobIds.ShowSyncId))
            {
                rows.Data = JsonConvert.DeserializeObject<List<ShowCollectionRow>>(statistic.JsonResult);
            }
            else
            {
                rows.Data = CalculateCollectedRows(libraryIds);
            }

            rows.TotalCount = rows.Data.Count();
            rows.Data = rows.Data.Skip(page * 30).Take(30);
            return rows;
        }

        public List<ShowCollectionRow> CalculateCollectedRows(string libraryId)
        {
            return CalculateCollectedRows(new List<string> { libraryId });
        }

        public List<ShowCollectionRow> CalculateCollectedRows(List<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);

            var stats = shows
                .Select(CreateShowCollectedRow)
                .OrderBy(x => x.SortName)
                .ToList();
            var json = JsonConvert.SerializeObject(stats);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowCollectedRows, libraryIds);

            return stats;
        }

        private ShowCollectionRow CreateShowCollectedRow(Show show)
        {
            var seasonCount = show.GetSeasonCount(false);

            return new ShowCollectionRow
            {
                Title = show.Name,
                SortName = show.SortName,
                Episodes = show.GetEpisodeCount(false, LocationType.Disk),
                Seasons = seasonCount,
                Specials = show.GetEpisodeCount(true, LocationType.Disk),
                MissingEpisodeCount = show.GetEpisodeCount(false, LocationType.Virtual),
                MissingEpisodes = show.GetMissingEpisodes().GroupBy(x => x.SeasonNumber, (index, episodes) => new VirtualSeason { Episodes = episodes, SeasonNumber = index }),
                PremiereDate = show.PremiereDate,
                Status = show.Status == "Continuing",
                Id = show.Id,
                Banner = show.Banner,
                Imdb = show.IMDB,
                Tvdb = show.TVDB,
                Size = show.GetShowSize()
            };
        }

        #endregion
    }
}
