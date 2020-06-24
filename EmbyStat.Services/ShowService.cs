using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Charts;
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

            if (StatisticsAreValid(statistic, libraryIds))
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
            var statistics = new ShowStatistics
            {
                General = CalculateGeneralStatistics(libraryIds),
                Charts = CalculateCharts(libraryIds),
                People = CalculatePeopleStatistics(libraryIds)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Show, libraryIds);

            return statistics;
        }

        public bool TypeIsPresent()
        {
            return _showRepository.Any();
        }

        #region General

        private ShowGeneral CalculateGeneralStatistics(IReadOnlyList<string> libraryIds)
        {
            return new ShowGeneral
            {
                ShowCount = TotalShowCount(libraryIds),
                EpisodeCount = TotalEpisodeCount(libraryIds),
                MissingEpisodeCount = TotalMissingEpisodeCount(libraryIds),
                TotalPlayableTime = CalculatePlayableTime(libraryIds),
                HighestRatedShow = CalculateHighestRatedShow(libraryIds),
                LowestRatedShow = CalculateLowestRatedShow(libraryIds),
                OldestPremieredShow = CalculateOldestPremieredShow(libraryIds),
                ShowWithMostEpisodes = CalculateShowWithMostEpisodes(libraryIds),
                LatestAddedShow = CalculateLatestAddedShow(libraryIds),
                NewestPremieredShow = CalculateNewestPremieredShow(libraryIds),
                TotalDiskSize = CalculateTotalDiskSize(libraryIds)
            };
        }

        private ShowPoster CalculateNewestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            //var newestPremiered = _showRepository.GetNewestPremieredMedia(libraryIds);
            //if (newestPremiered != null)
            //{
            //    return PosterHelper.ConvertToShowPoster(newestPremiered, Constants.Shows.NewestPremiered);
            //}

            return new ShowPoster();
        }

        private ShowPoster CalculateOldestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            //var oldestPremiered = _showRepository.GetOldestPremieredMedia(libraryIds);
            //if (oldestPremiered != null)
            //{
            //    return PosterHelper.ConvertToShowPoster(oldestPremiered, Constants.Shows.OldestPremiered);
            //}

            return new ShowPoster();
        }

        private ShowPoster CalculateLatestAddedShow(IReadOnlyList<string> libraryIds)
        {
            //var latestAdded = _showRepository.GetLatestAddedMedia(libraryIds);
            //if (latestAdded != null)
            //{
            //    return PosterHelper.ConvertToShowPoster(latestAdded, Constants.Shows.LatestAdded);
            //}

            return new ShowPoster();
        }

        private ShowPoster CalculateHighestRatedShow(IReadOnlyList<string> libraryIds)
        {
            //var highest = _showRepository.GetHighestRatedMedia(libraryIds);
            //if (highest != null)
            //{
            //    return PosterHelper.ConvertToShowPoster(highest, Constants.Shows.HighestRatedShow);
            //}

            return new ShowPoster();
        }

        private ShowPoster CalculateLowestRatedShow(IReadOnlyList<string> libraryIds)
        {
            //var lowest = _showRepository.GetLowestRatedMedia(libraryIds);
            //if (lowest != null)
            //{
            //    return PosterHelper.ConvertToShowPoster(lowest, Constants.Shows.LowestRatedShow);
            //}

            return new ShowPoster();
        }

        private ShowPoster CalculateShowWithMostEpisodes(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);

            var total = 0;
            Show resultShow = null;
            foreach (var show in shows)
            {
                var episodes = show.GetNonSpecialEpisodeCount(false);
                if (episodes > total)
                {
                    total = episodes;
                    resultShow = show;
                }
            }

            if (resultShow != null)
            {
                return PosterHelper.ConvertToShowPoster(resultShow, Constants.Shows.MostEpisodes);
            }

            return new ShowPoster();
        }

        private Card<int> TotalShowCount(IReadOnlyList<string> libraryIds)
        {
            var count = _showRepository.GetMediaCount(libraryIds);
            return new Card<int>
            {
                Title = Constants.Shows.TotalShows,
                Value = count
            };
        }

        private Card<int> TotalEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);
            return new Card<int>
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = shows.Sum(x => x.GetNonSpecialEpisodeCount(false))
            };
        }

        private Card<int> TotalMissingEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, false, true);
            var count = shows.Sum(x => x.MissingEpisodesCount);
            return new Card<int>
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = count
            };
        }

        private TimeSpanCard CalculatePlayableTime(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, false, false);
            var playLength = new TimeSpan(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
            return new TimeSpanCard
            {
                Title = Constants.Shows.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }

        private Card<double> CalculateTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, false, true);
            var episodes = shows.SelectMany(x => x.Episodes).Where(x => x.LocationType == LocationType.Disk);

            return CalculateTotalDiskSize(episodes);
        }

        #endregion

        #region Charts

        private ShowCharts CalculateCharts(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);

            var stats = new ShowCharts();
            stats.BarCharts.Add(CalculateGenreChart(shows));
            stats.BarCharts.Add(CalculateRatingChart(shows.Select(x => x.CommunityRating)));
            stats.BarCharts.Add(CalculatePremiereYearChart(shows.Select(x => x.PremiereDate)));
            stats.BarCharts.Add(CalculateCollectedRateChart(shows));
            stats.PieCharts.Add(CalculateOfficialRatingChart(shows));
            stats.PieCharts.Add(CalculateShowStateChart(shows));

            return stats;
        }

        private Chart CalculateShowStateChart(IReadOnlyList<Show> shows)
        {
            var list = shows
                .GroupBy(x => x.Status)
                .Select(x => new { Label = x.Key, Val0 = x.Count() })
                .OrderBy(x => x.Label)
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
                .OrderBy(x => x.Label)
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

        private PersonStats CalculatePeopleStatistics(IReadOnlyList<string> libraryIds)
        {
            return new PersonStats
            {
                MostFeaturedActorsPerGenre = GetMostFeaturedActorsPerGenreAsync(libraryIds)
            };
        }

        private List<PersonPoster> GetMostFeaturedActorsPerGenreAsync(IReadOnlyList<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, false, false).ToList();
            return GetMostFeaturedActorsPerGenre(shows);
        }

        #endregion

        #region Collected Rows

        public ListContainer<ShowCollectionRow> GetCollectedRows(List<string> libraryIds, int page)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCollectedRows, libraryIds);

            ListContainer<ShowCollectionRow> rows = new ListContainer<ShowCollectionRow>();
            if (StatisticsAreValid(statistic, libraryIds))
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
            var seasonCount = show.GetNonSpecialSeasonCount();

            return new ShowCollectionRow
            {
                Title = show.Name,
                SortName = show.SortName,
                Episodes = show.CollectedEpisodeCount,
                Seasons = seasonCount,
                Specials = show.SpecialEpisodeCount,
                MissingEpisodeCount = show.GetMissingEpisodeCount(),
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
