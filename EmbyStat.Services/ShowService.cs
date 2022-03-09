using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite.Shows;
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
            IStatisticsRepository statisticsRepository, ISettingsService settingsService) 
            : base(jobRepository, typeof(ShowService), "SHOW")
        {
            _showRepository = showRepository;
            _libraryRepository = libraryRepository;
            _statisticsRepository = statisticsRepository;
            _settingsService = settingsService;
        }

        public IEnumerable<Library> GetShowLibraries()
        {
            var settings = _settingsService.GetUserSettings();
            return _libraryRepository.GetLibrariesById(settings.ShowLibraries.Select(x => x.Id));
        }

        public async Task<ShowStatistics> GetStatistics(List<string> libraryIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Show, libraryIds);

            if (StatisticsAreValid(statistic, libraryIds, Constants.JobIds.ShowSyncId))
            {
                return JsonConvert.DeserializeObject<ShowStatistics>(statistic.JsonResult);
            }

            return await CalculateShowStatistics(libraryIds);
        }

        public async Task<ShowStatistics> CalculateShowStatistics(List<string> libraryIds)
        {
            var statistics = new ShowStatistics
            {
                Cards = await CalculateCards(libraryIds),
                TopCards = CalculateTopCards(libraryIds),
                BarCharts = await CalculateBarCharts(libraryIds),
                PieCharts = await CalculatePieChars(libraryIds)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Show, libraryIds);

            return statistics;
        }

        public bool TypeIsPresent()
        {
            return _showRepository.Any();
        }

        public async Task<Page<SqlShow>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount, List<string> libraryIds)
        {
            var list = await _showRepository.GetShowPage(skip, take, sortField, sortOrder, filters, libraryIds);

            var page = new Page<SqlShow>(list);
            if (requireTotalCount)
            {
                page.TotalCount = await _showRepository.Count(filters, libraryIds);
            }

            return page;
        }

        public Task<SqlShow> GetShow(string id)
        {
            return _showRepository.GetShowByIdWithEpisodes(id);
        }

        #region Cards

        private async Task<List<Card<string>>> CalculateCards(IReadOnlyList<string> libraryIds)
        {
            var list = new List<Card<string>>();
            list.AddIfNotNull(await CalculateTotalShowCount(libraryIds));
            list.AddIfNotNull(await CalculateCompleteCollectedShowCount(libraryIds));
            list.AddIfNotNull(await CalculateTotalEpisodeCount(libraryIds));
            list.AddIfNotNull(await CalculateTotalMissingEpisodeCount(libraryIds));
            list.AddIfNotNull(await CalculateTotalShowGenres(libraryIds));
            list.AddIfNotNull(await CalculatePlayableTime(libraryIds));
            list.AddIfNotNull(await CalculateTotalDiskSpace(libraryIds));
            list.AddIfNotNull(TotalPersonTypeCount(libraryIds, PersonType.Actor, Constants.Common.TotalActors));

            return list;
        }

        private Task<Card<string>> CalculateTotalShowCount(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var count = await _showRepository.Count(libraryIds);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalShows,
                    Value = count.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total show count failed:");
        }

        private Task<Card<string>> CalculateCompleteCollectedShowCount(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var count = await _showRepository.CompleteCollectedCount(libraryIds);
                return new Card<string>
                {
                    Title = Constants.Shows.TotalCompleteCollectedShows,
                    Value = count.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total completed collected show count failed:");
        }

        private Task<Card<string>> CalculateTotalEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetEpisodeCount(libraryIds, LocationType.Disk);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalEpisodes,
                    Value = total.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total episode count failed:");
        }

        private Task<Card<string>> CalculateTotalShowGenres(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var totalGenres = await _showRepository.GetGenreCount(libraryIds);
                return new Card<string>
                {
                    Title = Constants.Common.TotalGenres,
                    Value = totalGenres.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.PoundRoundedIcon
                };
            }, "Calculate total show genres count failed:");
        }

        private Task<Card<string>> CalculateTotalMissingEpisodeCount(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetEpisodeCount(libraryIds, LocationType.Virtual);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalMissingEpisodes,
                    Value = total.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total missing episodes failed:");
        }

        private Task<Card<string>> CalculatePlayableTime(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var totalRunTimeTicks = await _showRepository.GetTotalRunTimeTicks(libraryIds);
                var playLength = new TimeSpan(totalRunTimeTicks);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalPlayLength,
                    Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                    Type = CardType.Time,
                    Icon = Constants.Icons.QueryBuilderRoundedIcon
                };
            }, "Calculate total playable time failed:");
        }

        private Task<Card<string>> CalculateTotalDiskSpace(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetTotalDiskSpaceUsed(libraryIds);

                return new Card<string>
                {
                    Value = total.ToString(CultureInfo.InvariantCulture),
                    Title = Constants.Common.TotalDiskSpace,
                    Type = CardType.Size,
                    Icon = Constants.Icons.StorageRoundedIcon
                };
            }, "Calculate total disk space failed:");
        }
        
        private Card<string> TotalPersonTypeCount(IReadOnlyList<string> libraryIds, PersonType type, string title)
        {
            return CalculateStat(() =>
            {
                var value = _showRepository.GetPeopleCount(libraryIds, type);
                return new Card<string>
                {
                    Value = value.ToString(),
                    Title = title,
                    Icon = Constants.Icons.PeopleAltRoundedIcon,
                    Type = CardType.Text
                };
            }, $"Calculate total {type} count failed::");
        }

        #endregion

        #region TopCards

        private List<TopCard> CalculateTopCards(IReadOnlyList<string> libraryIds)
        {
            var list = new List<TopCard>();
            list.AddIfNotNull(CalculateNewestPremieredShow(libraryIds));
            list.AddIfNotNull(CalculateOldestPremieredShow(libraryIds));
            list.AddIfNotNull(CalculateLatestAddedShow(libraryIds));
            list.AddIfNotNull(CalculateHighestRatedShow(libraryIds));
            list.AddIfNotNull(CalculateLowestRatedShow(libraryIds));
            list.AddIfNotNull(CalculateShowWithMostEpisodes(libraryIds));

            return list;
        }

        private TopCard CalculateNewestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetNewestPremieredMedia(libraryIds, 5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                    : null;
            }, "Calculate newest premiered shows failed:");
        }

        private TopCard CalculateOldestPremieredShow(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetOldestPremieredMedia(libraryIds, 5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                    : null;
            }, "Calculate oldest premiered shows failed:");
        }

        private TopCard CalculateLatestAddedShow(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetLatestAddedMedia(libraryIds, 5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.LatestAdded, "COMMON.DATE", "DateCreated", ValueTypeEnum.Date)
                    : null;
            }, "Calculate latest added shows failed:");
        }

        private TopCard CalculateHighestRatedShow(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetHighestRatedMedia(libraryIds, 5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.HighestRatedShow, "/10", "CommunityRating", false)
                    : null;
            }, "Calculate highest rated shows failed:");
        }

        private TopCard CalculateLowestRatedShow(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetLowestRatedMedia(libraryIds, 5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.LowestRatedShow, "/10", "CommunityRating", false)
                    : null;
            }, "Calculate lowest rated shows failed:");
        }

        private TopCard CalculateShowWithMostEpisodes(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetShowsWithMostEpisodes(libraryIds, 5);

                return list.Count > 0
                    ? new TopCard()
                    : null;
            }, "Calculate shows with most episodes failed:");
        }

        #endregion

        #region Charts

        private async Task<List<Chart>> CalculateBarCharts(IReadOnlyList<string> libraryIds)
        {
            var list = new List<Chart>();
            list.AddIfNotNull(await CalculateGenreChart(libraryIds));
            list.AddIfNotNull(CalculateRatingChart(libraryIds));
            list.AddIfNotNull(CalculatePremiereYearChart(libraryIds));
            list.AddIfNotNull(await CalculateCollectedRateChart(libraryIds));
            list.AddIfNotNull(await CalculateOfficialRatingChart(libraryIds));
            return list;
        }

        private Task<Chart> CalculateGenreChart(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var genres = await _showRepository.GetGenreChartValues(libraryIds);
                return CreateGenreChart(genres);
            }, "Calculate genre chart failed:");
        }

        private Chart CalculateRatingChart(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var items = _showRepository.GetCommunityRatings(libraryIds);
                return CreateRatingChart(items);
            }, "Calculate rating chart failed:");
        }

        private Chart CalculatePremiereYearChart(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(() =>
            {
                var yearDataList = _showRepository.GetPremiereYears(libraryIds);
                return CalculatePremiereYearChart(yearDataList);
            }, "Calculate premiered year chart failed:");
        }

        private Task<Chart> CalculateOfficialRatingChart(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var ratings = await _showRepository.GetOfficialRatingChartValues(libraryIds);
                return CalculateOfficialRatingChart(ratings);
            }, "Calculate official movie rating chart failed:");
        }

        private async Task<List<Chart>> CalculatePieChars(IReadOnlyList<string> libraryIds)
        {
            var list = new List<Chart>();
            list.AddIfNotNull(await CalculateShowStateChart(libraryIds));

            return list;
        }

        private Task<Chart> CalculateShowStateChart(IReadOnlyList<string> libraryIds)
        {
            return CalculateStat(async () =>
            {
                var list = await _showRepository.GetShowStatusCharValues(libraryIds);
                var results = list
                    .Select(x => new SimpleChartData { Label = x.Key, Value=x.Value })
                    .OrderByDescending(x => x.Value)
                    .ToArray();

                return new Chart
                {
                    Title = Constants.Shows.ShowStatusChart,
                    DataSets = results,
                    SeriesCount = 1

                };
            }, "Calculate show state chart failed:");
        }

        private async Task<Chart> CalculateCollectedRateChart(IReadOnlyList<string> libraryIds)
        {
            var percentageList = await _showRepository.GetCollectedRateChart(libraryIds);

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
                .Select(x => new SimpleChartData { Label = x.Key != 100 ? $"{x.Key}% - {x.Key + 4}%" : $"{x.Key}%", Value = x.Count() })
                .ToArray();

            return new Chart
            {
                Title = Constants.CountPerCollectedPercentage,
                DataSets = rates,
                SeriesCount = 1
            };
        }

        #endregion


        #region Collected Rows

        public async Task<ListContainer<ShowCollectionRow>> GetCollectedRows(IReadOnlyList<string> libraryIds, int page)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCollectedRows, libraryIds);

            ListContainer<ShowCollectionRow> rows = new ListContainer<ShowCollectionRow>();
            if (StatisticsAreValid(statistic, libraryIds, Constants.JobIds.ShowSyncId))
            {
                rows.Data = JsonConvert.DeserializeObject<List<ShowCollectionRow>>(statistic.JsonResult);
            }
            else
            {
                rows.Data = await CalculateCollectedRows(libraryIds);
            }

            rows.TotalCount = rows.Data.Count();
            rows.Data = rows.Data.Skip(page * 30).Take(30);
            return rows;
        }

        public Task<List<ShowCollectionRow>> CalculateCollectedRows(string libraryId)
        {
            return CalculateCollectedRows(new List<string> { libraryId });
        }

        public async Task<List<ShowCollectionRow>> CalculateCollectedRows(IReadOnlyList<string> libraryIds)
        {
            var shows = await _showRepository.GetAllShowsWithEpisodes(libraryIds);

            //var stats = shows
            //    .Select(CreateShowCollectedRow)
            //    .OrderBy(x => x.SortName)
            //    .ToList();
            //var json = JsonConvert.SerializeObject(stats);
            //_statisticsRepository.AddStatistic(json, DateTimeUtc.UtcNow, StatisticType.ShowCollectedRows, libraryIds);

            return new List<ShowCollectionRow>();
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
                PremiereDate = show.PremiereDate,
                Status = show.Status == "Continuing",
                Id = show.Id,
                Banner = show.Banner,
                Imdb = show.IMDB,
                Tvdb = show.TVDB,
                Size = show.SizeInMb
            };
        }

        #endregion
    }
}
