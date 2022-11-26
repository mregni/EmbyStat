using System.Globalization;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Abstract;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace EmbyStat.Core.Shows;

public class ShowService : MediaService, IShowService
{
    private readonly IShowRepository _showRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMediaServerRepository _mediaServerRepository;

    public ShowService(IJobRepository jobRepository, IShowRepository showRepository,
        IStatisticsRepository statisticsRepository, IMediaServerRepository mediaServerRepository,
        ILogger<ShowService> logger, IConfigurationService configurationService) 
        : base(jobRepository, logger, configurationService)
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
        var statisticsJson = await _statisticsRepository.GetLastResultByType(StatisticType.Show);

        ShowStatistics statistics;
        if (StatisticsAreValid(statisticsJson, Constants.JobIds.ShowSyncId))
        {
            statistics = JsonConvert.DeserializeObject<ShowStatistics>(statisticsJson.JsonResult) ?? new ShowStatistics();
        }
        else
        {
            statistics = await CalculateShowStatistics();
        }

        await AddLiveStatistics(statistics);

        return statistics;
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
        await _statisticsRepository.ReplaceStatistic(json, DateTime.UtcNow, StatisticType.Show);

        return statistics;
    }

    private async Task AddLiveStatistics(ShowStatistics statistics)
    {
        statistics.ComplexCharts = await CalculateComplexCharts();
        statistics.Cards.AddIfNotNull(await GetCurrentWatchingCount(_showRepository.GetCurrentWatchingCount,Constants.Shows.CurrentPlayingCount));
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
        
    public async Task SetLibraryAsSynced(string[] libraryIds)
    {
        await _mediaServerRepository.SetLibraryAsSynced(libraryIds, LibraryType.TvShow);
        await _showRepository.RemoveUnwantedShows(libraryIds);
        await _statisticsRepository.MarkTypesAsInvalid(StatisticType.Show);
    }

    #region Cards

    private async Task<List<Card>> CalculateCards()
    {
        var list = new List<Card>();
        list.AddIfNotNull(await CalculateTotalShowCount());
        list.AddIfNotNull(await CalculateCompleteCollectedShowCount());
        list.AddIfNotNull(await CalculateTotalEpisodeCount());
        list.AddIfNotNull(await CalculateTotalMissingEpisodeCount());
        list.AddIfNotNull(await CalculateTotalShowGenres());
        list.AddIfNotNull(await CalculatePlayableTime());
        list.AddIfNotNull(await CalculateTotalDiskSpace());
        list.AddIfNotNull(CalculateWatchedEpisodeCount());
        list.AddIfNotNull(await CalculateTotalWatchedTime());
        list.AddIfNotNull(TotalPersonTypeCount(PersonType.Actor, Constants.Common.TotalActors));

        return list;
    }

    private Task<Card> CalculateTotalShowCount()
    {
        return CalculateStat(async () =>
        {
            var count = await _showRepository.Count();

            return new Card
            {
                Title = Constants.Shows.TotalShows,
                Value = count.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }, "Calculate total show count failed:");
    }

    private Task<Card> CalculateCompleteCollectedShowCount()
    {
        return CalculateStat(async () =>
        {
            var count = await _showRepository.CompleteCollectedCount();
            return new Card
            {
                Title = Constants.Shows.TotalCompleteCollectedShows,
                Value = count.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }, "Calculate total completed collected show count failed:");
    }

    private Task<Card> CalculateTotalEpisodeCount()
    {
        return CalculateStat(async () =>
        {
            var total = await _showRepository.GetEpisodeCount(LocationType.Disk);

            return new Card
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = total.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }, "Calculate total episode count failed:");
    }

    private Task<Card> CalculateTotalShowGenres()
    {
        return CalculateStat(async () =>
        {
            var totalGenres = await _showRepository.GetGenreCount();
            return new Card
            {
                Title = Constants.Common.TotalGenres,
                Value = totalGenres.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }, "Calculate total show genres count failed:");
    }

    private Task<Card> CalculateTotalMissingEpisodeCount()
    {
        return CalculateStat(async () =>
        {
            var total = await _showRepository.GetEpisodeCount(LocationType.Virtual);

            return new Card
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = total.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }, "Calculate total missing episodes failed:");
    }

    private Task<Card> CalculatePlayableTime()
    {
        return CalculateStat(async () =>
        {
            var totalRunTimeTicks = await _showRepository.GetTotalRunTimeTicks();
            var playLength = new TimeSpan(totalRunTimeTicks);

            return new Card
            {
                Title = Constants.Shows.TotalPlayLength,
                Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                Type = CardType.Time,
                Icon = Constants.Icons.QueryBuilderRoundedIcon
            };
        }, "Calculate total playable time failed:");
    }

    private Task<Card> CalculateTotalDiskSpace()
    {
        return CalculateStat(async () =>
        {
            var total = await _showRepository.GetTotalDiskSpaceUsed();

            return new Card
            {
                Value = total.ToString(CultureInfo.InvariantCulture),
                Title = Constants.Common.TotalDiskSpace,
                Type = CardType.Size,
                Icon = Constants.Icons.StorageRoundedIcon
            };
        }, "Calculate total disk space failed:");
    }
        
    private Card TotalPersonTypeCount(PersonType type, string title)
    {
        return CalculateStat(() =>
        {
            var value = _showRepository.GetPeopleCount(type);
            return new Card
            {
                Value = value.ToString(),
                Title = title,
                Icon = Constants.Icons.PeopleAltRoundedIcon,
                Type = CardType.Text
            };
        }, $"Calculate total {type} count failed::");
    }
    
    private Card CalculateWatchedEpisodeCount()
    {
        return CalculateStat(() =>
        {
            var viewCount = _showRepository.GetTotalWatchedEpisodeCount();
            return new Card
            {
                Title = Constants.Shows.TotalWatchedEpisodes,
                Value = viewCount.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }, "Calculate total watched episode count failed:");
    }
    
    private Task<Card> CalculateTotalWatchedTime()
    {
        return CalculateStat(async () =>
        {
            var playLengthTicks = await _showRepository.GetPlayedRuntime();
            var playLength = new TimeSpan(playLengthTicks);

            return new Card
            {
                Title = Constants.Shows.PlayedPlayLength,
                Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                Type = CardType.Time,
                Icon = Constants.Icons.QueryBuilderRoundedIcon
            };
        }, "Calculate played show play length failed:");
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
        list.AddIfNotNull(await CalculateMostWatchedShows());

        return list;
    }

    private Task<TopCard?> CalculateNewestPremieredShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetNewestPremieredMedia(5);
            var list = data.ToArray();
                
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                : null;
        }, "Calculate newest premiered shows failed:");
    }

    private Task<TopCard?> CalculateOldestPremieredShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetOldestPremieredMedia(5);
            var list = data.ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                : null;
        }, "Calculate oldest premiered shows failed:");
    }

    private TopCard? CalculateLatestAddedShow()
    {
        return CalculateStat(() =>
        {
            var list = _showRepository.GetLatestAddedMedia(5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.LatestAdded, "COMMON.DATE", "DateCreated", ValueType.Date)
                : null;
        }, "Calculate latest added shows failed:");
    }

    private Task<TopCard?> CalculateHighestRatedShow()
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

    private Task<TopCard?> CalculateLowestRatedShow()
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

    private Task<TopCard?> CalculateShowWithMostEpisodes()
    {
        return CalculateStat(async () =>
        {
            var list = await _showRepository.GetShowsWithMostEpisodes(5);

            return list.Count > 0
                ? list.ConvertToTopCard(Constants.Shows.MostEpisodes, "#", false)
                : null;
        }, "Calculate shows with most episodes failed:");
    }
        
    private TopCard? CalculateMostDiskSpaceUsedShow()
    {
        return CalculateStat(() =>
        {
            var list = _showRepository.GetShowsWithMostDiskSpaceUsed(5).ToArray();

            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Shows.MostDiskSpace, "#", false, ValueType.SizeInMb)
                : null;
        }, "Calculate shows with most episodes failed:");
    }
    
    private Task<TopCard?> CalculateMostWatchedShows()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetMostWatchedShows(5);

            return data.Count > 0
                ? data.ConvertToTopCard(Constants.Shows.MostWatchedShows, "#", false)
                : null;
        }, "Calculate most watched shows failed:");
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

    #region Complex charts

    private async Task<List<MultiChart>> CalculateComplexCharts()
    {
        var list = new List<MultiChart>();
        
        var watchedPerHour = _showRepository.GetWatchedPerHourOfDayChartValues;
        list.AddIfNotNull(await CalculateWatchedPerHourOfDayChart(watchedPerHour, Constants.Shows.WatchedPerHour));
        
        var wachtedPerDay = _showRepository.GetWatchedPerDayOfWeekChartValues;
        list.AddIfNotNull(await CalculateWatchedPerDayOfWeekChart(wachtedPerDay, Constants.Shows.DaysOfTheWeek));

        return list;
    }

    #endregion
}