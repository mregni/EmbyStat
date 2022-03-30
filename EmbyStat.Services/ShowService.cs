using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
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
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMediaServerRepository _mediaServerRepository;

        public ShowService(IJobRepository jobRepository, IShowRepository showRepository,
            IStatisticsRepository statisticsRepository, IMediaServerRepository mediaServerRepository) 
            : base(jobRepository, typeof(ShowService), "SHOW")
        {
            _showRepository = showRepository;
            _statisticsRepository = statisticsRepository;
            _mediaServerRepository = mediaServerRepository;
        }

        public Task<List<Library>> GetShowLibraries()
        {
            return _mediaServerRepository.GetAllLibraries(LibraryType.TvShow);
        }

        public async Task<ShowStatistics> GetStatistics()
        {
            var statistic = await _statisticsRepository.GetLastResultByType(StatisticType.Show);

            if (StatisticsAreValid(statistic, Constants.JobIds.ShowSyncId))
            {
                return JsonConvert.DeserializeObject<ShowStatistics>(statistic.JsonResult);
            }

            return await CalculateShowStatistics();
        }

        public async Task<ShowStatistics> CalculateShowStatistics()
        {
            var statistics = new ShowStatistics
            {
                Cards = await CalculateCards(),
                TopCards = await CalculateTopCards(),
                BarCharts = await CalculateBarCharts(),
                PieCharts = await CalculatePieChars()
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.ReplaceStatistic(json, DateTime.UtcNow, StatisticType.Show);

            return statistics;
        }

        public bool TypeIsPresent()
        {
            return _showRepository.Any();
        }

        public async Task<Page<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount)
        {
            var list = await _showRepository.GetShowPage(skip, take, sortField, sortOrder, filters);

            var page = new Page<Show>(list);
            if (requireTotalCount)
            {
                page.TotalCount = await _showRepository.Count(filters);
            }

            return page;
        }

        public Task<Show> GetShow(string id)
        {
            return _showRepository.GetShowByIdWithEpisodes(id);
        }
        
        public Task UpdateLibraries(string[] libraryIds)
        {
            return _mediaServerRepository.SetLibraryAsSynced(libraryIds, LibraryType.TvShow);
        }

        #region Cards

        private async Task<List<Card<string>>> CalculateCards()
        {
            var list = new List<Card<string>>();
            list.AddIfNotNull(await CalculateTotalShowCount());
            list.AddIfNotNull(await CalculateCompleteCollectedShowCount());
            list.AddIfNotNull(await CalculateTotalEpisodeCount());
            list.AddIfNotNull(await CalculateTotalMissingEpisodeCount());
            list.AddIfNotNull(await CalculateTotalShowGenres());
            list.AddIfNotNull(await CalculatePlayableTime());
            list.AddIfNotNull(await CalculateTotalDiskSpace());
            list.AddIfNotNull(TotalPersonTypeCount(PersonType.Actor, Constants.Common.TotalActors));

            return list;
        }

        private Task<Card<string>> CalculateTotalShowCount()
        {
            return CalculateStat(async () =>
            {
                var count = await _showRepository.Count();

                return new Card<string>
                {
                    Title = Constants.Shows.TotalShows,
                    Value = count.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total show count failed:");
        }

        private Task<Card<string>> CalculateCompleteCollectedShowCount()
        {
            return CalculateStat(async () =>
            {
                var count = await _showRepository.CompleteCollectedCount();
                return new Card<string>
                {
                    Title = Constants.Shows.TotalCompleteCollectedShows,
                    Value = count.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total completed collected show count failed:");
        }

        private Task<Card<string>> CalculateTotalEpisodeCount()
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetEpisodeCount(LocationType.Disk);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalEpisodes,
                    Value = total.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total episode count failed:");
        }

        private Task<Card<string>> CalculateTotalShowGenres()
        {
            return CalculateStat(async () =>
            {
                var totalGenres = await _showRepository.GetGenreCount();
                return new Card<string>
                {
                    Title = Constants.Common.TotalGenres,
                    Value = totalGenres.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.PoundRoundedIcon
                };
            }, "Calculate total show genres count failed:");
        }

        private Task<Card<string>> CalculateTotalMissingEpisodeCount()
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetEpisodeCount(LocationType.Virtual);

                return new Card<string>
                {
                    Title = Constants.Shows.TotalMissingEpisodes,
                    Value = total.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                };
            }, "Calculate total missing episodes failed:");
        }

        private Task<Card<string>> CalculatePlayableTime()
        {
            return CalculateStat(async () =>
            {
                var totalRunTimeTicks = await _showRepository.GetTotalRunTimeTicks();
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

        private Task<Card<string>> CalculateTotalDiskSpace()
        {
            return CalculateStat(async () =>
            {
                var total = await _showRepository.GetTotalDiskSpaceUsed();

                return new Card<string>
                {
                    Value = total.ToString(CultureInfo.InvariantCulture),
                    Title = Constants.Common.TotalDiskSpace,
                    Type = CardType.Size,
                    Icon = Constants.Icons.StorageRoundedIcon
                };
            }, "Calculate total disk space failed:");
        }
        
        private Card<string> TotalPersonTypeCount(PersonType type, string title)
        {
            return CalculateStat(() =>
            {
                var value = _showRepository.GetPeopleCount(type);
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

        private async Task<List<TopCard>> CalculateTopCards()
        {
            var list = new List<TopCard>();
            list.AddIfNotNull(await CalculateNewestPremieredShow());
            list.AddIfNotNull(await CalculateOldestPremieredShow());
            list.AddIfNotNull(CalculateLatestAddedShow());
            list.AddIfNotNull(await CalculateHighestRatedShow());
            list.AddIfNotNull(await CalculateLowestRatedShow());
            list.AddIfNotNull(await CalculateShowWithMostEpisodes());
            list.AddIfNotNull(CalculateMostDiskSpaceUsedShow());

            return list;
        }

        private Task<TopCard> CalculateNewestPremieredShow()
        {
            return CalculateStat(async () =>
            {
                var data = await _showRepository.GetNewestPremieredMedia(5);
                var list = data.ToArray();
                
                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                    : null;
            }, "Calculate newest premiered shows failed:");
        }

        private Task<TopCard> CalculateOldestPremieredShow()
        {
            return CalculateStat(async () =>
            {
                var data = await _showRepository.GetOldestPremieredMedia(5);
                var list = data.ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueTypeEnum.Date)
                    : null;
            }, "Calculate oldest premiered shows failed:");
        }

        private TopCard CalculateLatestAddedShow()
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetLatestAddedMedia(5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.LatestAdded, "COMMON.DATE", "DateCreated", ValueTypeEnum.Date)
                    : null;
            }, "Calculate latest added shows failed:");
        }

        private Task<TopCard> CalculateHighestRatedShow()
        {
            return CalculateStat(async () =>
            {
                var data = await _showRepository.GetHighestRatedMedia(5);
                var list = data.ToArray();
                
                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.HighestRatedShow, "/10", "CommunityRating", false)
                    : null;
            }, "Calculate highest rated shows failed:");
        }

        private Task<TopCard> CalculateLowestRatedShow()
        {
            return CalculateStat(async () =>
            {
                var data = await _showRepository.GetLowestRatedMedia(5);
                var list = data.ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.LowestRatedShow, "/10", "CommunityRating", false)
                    : null;
            }, "Calculate lowest rated shows failed:");
        }

        private Task<TopCard> CalculateShowWithMostEpisodes()
        {
            return CalculateStat(async () =>
            {
                var list = await _showRepository.GetShowsWithMostEpisodes(5);

                return list.Count > 0
                    ? list.ConvertToTopCard(Constants.Shows.MostEpisodes, "#", false)
                    : null;
            }, "Calculate shows with most episodes failed:");
        }
        
        private TopCard CalculateMostDiskSpaceUsedShow()
        {
            return CalculateStat(() =>
            {
                var list = _showRepository.GetShowsWithMostDiskSpaceUsed(5).ToArray();

                return list.Length > 0
                    ? list.ConvertToTopCard(Constants.Shows.MostDiskSpace, "#", false, ValueTypeEnum.SizeInMb)
                    : null;
            }, "Calculate shows with most episodes failed:");
        }

        #endregion

        #region Charts

        private async Task<List<Chart>> CalculateBarCharts()
        {
            var list = new List<Chart>();
            list.AddIfNotNull(await CalculateGenreChart());
            list.AddIfNotNull(CalculateRatingChart());
            list.AddIfNotNull(CalculatePremiereYearChart());
            list.AddIfNotNull(await CalculateCollectedRateChart());
            list.AddIfNotNull(await CalculateOfficialRatingChart());
            return list;
        }

        private Task<Chart> CalculateGenreChart()
        {
            return CalculateStat(async () =>
            {
                var genres = await _showRepository.GetGenreChartValues();
                return CreateGenreChart(genres);
            }, "Calculate genre chart failed:");
        }

        private Chart CalculateRatingChart()
        {
            return CalculateStat(() =>
            {
                var items = _showRepository.GetCommunityRatings();
                return CreateRatingChart(items);
            }, "Calculate rating chart failed:");
        }

        private Chart CalculatePremiereYearChart()
        {
            return CalculateStat(() =>
            {
                var yearDataList = _showRepository.GetPremiereYears();
                return CalculatePremiereYearChart(yearDataList);
            }, "Calculate premiered year chart failed:");
        }

        private Task<Chart> CalculateOfficialRatingChart()
        {
            return CalculateStat(async () =>
            {
                var ratings = await _showRepository.GetOfficialRatingChartValues();
                return CalculateOfficialRatingChart(ratings);
            }, "Calculate official movie rating chart failed:");
        }

        private async Task<List<Chart>> CalculatePieChars()
        {
            var list = new List<Chart>();
            list.AddIfNotNull(await CalculateShowStateChart());

            return list;
        }

        private Task<Chart> CalculateShowStateChart()
        {
            return CalculateStat(async () =>
            {
                var list = await _showRepository.GetShowStatusCharValues();
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

        private async Task<Chart> CalculateCollectedRateChart()
        {
            var percentageList = await _showRepository.GetCollectedRateChart();

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
    }
}
