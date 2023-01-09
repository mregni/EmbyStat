using System.Globalization;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.Extensions.Logging;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace EmbyStat.Core.Statistics;

public class ShowStatisticService : MediaStatisticService, IShowStatisticsService
{
    private readonly IShowRepository _showRepository;

    public ShowStatisticService(ILogger<ShowStatisticService> logger, IConfigurationService configurationService,
        IShowRepository showRepository)
        : base(logger, configurationService)
    {
        _showRepository = showRepository;
    }

    public Task<string> CalculateStatistic(StatisticCardType cardType, Statistic uniqueType)
    {
        switch (cardType)
        {
            case StatisticCardType.Card:
                return CalculateCard(uniqueType);
            case StatisticCardType.TopCard:
                return CalculateTopCard(uniqueType);
            case StatisticCardType.BarChart:
                return CalculateBarChart(uniqueType);
            case StatisticCardType.PieChart:
                return CalculatePieChart(uniqueType);
            case StatisticCardType.ComplexChart:
                return CalculateComplexChart(uniqueType);
        }

        return Task.FromResult((string?) null)!;
    }

    #region Cards

    private Task<string> CalculateCard(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.ShowTotalCount => CalculateTotalShowCount(),
            Statistic.ShowTotalGenreCount => CalculateTotalShowGenres(),
            Statistic.ShowCompleteCollectedCount => CalculateCompleteCollectedShowCount(),
            Statistic.ShowTotalEpisodesCount => CalculateTotalEpisodeCount(),
            Statistic.ShowTotalMissingEpisodeCount => CalculateTotalMissingEpisodeCount(),
            Statistic.ShowTotalPlayLength => CalculatePlayableTime(),
            Statistic.ShowTotalDiskSpaceUsage => CalculateTotalDiskSpace(),
            Statistic.ShowTotalWatchedEpisodeCount => CalculateWatchedEpisodeCount(),
            Statistic.ShowTotalWatchedTime => CalculateTotalWatchedTime(),
            Statistic.ShowTotalActorCount => TotalPersonTypeCount(PersonType.Actor, Constants.Common.TotalActors),
            _ => Task.FromResult((string) null!)
        };
    }

    private Task<string> CalculateTotalShowCount()
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
            }.BuildJson();
        }, "Calculate total show count failed:");
    }
    
    private Task<string> CalculateTotalShowGenres()
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
            }.BuildJson();
        }, "Calculate total show genres count failed:");
    }
    
    private Task<string> CalculateCompleteCollectedShowCount()
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
            }.BuildJson();
        }, "Calculate total completed collected show count failed:");
    }
    
    private Task<string> CalculateTotalEpisodeCount()
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
            }.BuildJson();
        }, "Calculate total episode count failed:");
    }
    
    private Task<string> CalculateTotalMissingEpisodeCount()
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
            }.BuildJson();
        }, "Calculate total missing episodes failed:");
    }
    
    private Task<string> CalculatePlayableTime()
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
            }.BuildJson();
        }, "Calculate total playable time failed:");
    }
    
    private Task<string> CalculateTotalDiskSpace()
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
            }.BuildJson();
        }, "Calculate total disk space failed:");
    }
    
    private Task<string> CalculateWatchedEpisodeCount()
    {
        return CalculateStat(async () =>
        {
            var viewCount = await _showRepository.GetTotalWatchedEpisodeCount();
            return new Card
            {
                Title = Constants.Shows.TotalWatchedEpisodes,
                Value = viewCount.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            }.BuildJson();
        }, "Calculate total watched episode count failed:");
    }
    
    private Task<string> CalculateTotalWatchedTime()
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
            }.BuildJson();
        }, "Calculate played show play length failed:");
    }
    
    private Task<string> TotalPersonTypeCount(PersonType type, string title)
    {
        return CalculateStat(async () =>
        {
            var value = await _showRepository.GetPeopleCount(type);
            return new Card
            {
                Value = value.ToString(),
                Title = title,
                Icon = Constants.Icons.PeopleAltRoundedIcon,
                Type = CardType.Text
            }.BuildJson();
        }, $"Calculate total {type} count failed::");
    }

    #endregion

    #region TopCards

    private Task<string> CalculateTopCard(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.ShowNewestPremieredList => CalculateNewestPremieredShow(),
            Statistic.ShowOldestPremieredList => CalculateOldestPremieredShow(),
            Statistic.ShowLatestAddedList => CalculateLatestAddedShow(),
            Statistic.ShowHighestRatedList => CalculateHighestRatedShow(),
            Statistic.ShowLowestRatedList => CalculateLowestRatedShow(),
            Statistic.ShowWithMostEpisodesList => CalculateShowWithMostEpisodes(),
            Statistic.ShowMostDiskUsageList => CalculateMostDiskSpaceUsedShow(),
            Statistic.ShowMostWatchedList => CalculateMostWatchedShows(),
            _ => Task.FromResult((string) null!)
        };
    }
    
    private Task<string> CalculateNewestPremieredShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetNewestPremieredMedia(5);
            var list = data.ToArray();
                
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate newest premiered shows failed:");
    }
    
    private Task<string> CalculateOldestPremieredShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetOldestPremieredMedia(5);
            var list = data.ToArray();

            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate oldest premiered shows failed:");
    }
    
    private Task<string> CalculateLatestAddedShow()
    {
        return CalculateStat(async () =>
        {
            var list = await _showRepository.GetLatestAddedShows(5);
    
            return list.Count > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.LatestAdded, "COMMON.DATE", "DateCreated", ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate latest added shows failed:");
    }
    
    private Task<string> CalculateHighestRatedShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetHighestRatedMedia(5);
            var list = data.ToArray();
                
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.HighestRatedShow, "/10", "CommunityRating", false)
                    .BuildJson()
                : null!;
        }, "Calculate highest rated shows failed:");
    }
    
    private Task<string> CalculateLowestRatedShow()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetLowestRatedMedia(5);
            var list = data.ToArray();

            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.LowestRatedShow, "/10", "CommunityRating", false)
                    .BuildJson()
                : null!;
        }, "Calculate lowest rated shows failed:");
    }
    
    private Task<string> CalculateShowWithMostEpisodes()
    {
        return CalculateStat(async () =>
        {
            var list = await _showRepository.GetShowsWithMostEpisodes(5);

            return list.Count > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.MostEpisodes, "#", false)
                    .BuildJson()
                : null!;
        }, "Calculate shows with most episodes failed:");
    }
    
    private Task<string> CalculateMostDiskSpaceUsedShow()
    {
        return CalculateStat(async () =>
        {
            var list = await _showRepository.GetShowsWithMostDiskSpaceUsed(5);

            return list.Count > 0
                ? list
                    .ConvertToTopCard(Constants.Shows.MostDiskSpace, "#", false, ValueType.SizeInMb)
                    .BuildJson()
                : null!;
        }, "Calculate shows with most episodes failed:");
    }
    
    private Task<string> CalculateMostWatchedShows()
    {
        return CalculateStat(async () =>
        {
            var data = await _showRepository.GetMostWatchedShows(5);

            return data.Count > 0
                ? data
                    .ConvertToTopCard(Constants.Shows.MostWatchedShows, "#", false)
                    .BuildJson()
                : null!;
        }, "Calculate most watched shows failed:");
    }

    #endregion

    #region BarCharts

    private Task<string> CalculateBarChart(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.ShowGenreChart => CalculateGenreChart(),
            Statistic.ShowRatingChart => CalculateRatingChart(),
            Statistic.ShowPremiereYearChart => CalculatePremiereYearChart(),
            Statistic.ShowCollectedRateChart => CalculateCollectedRateChart(),
            Statistic.ShowOfficialRatingChart => CalculateOfficialRatingChart(),
            _ => Task.FromResult((string) null!)
        };
    }
    
    private Task<string> CalculateGenreChart()
    {
        return CalculateStat(async () =>
        {
            var genres = await _showRepository.GetGenreChartValues();
            return CreateGenreChart(genres).BuildJson();
        }, "Calculate genre chart failed:");
    }

    private Task<string> CalculateRatingChart()
    {
        return CalculateStat(async () =>
        {
            var items = await _showRepository.GetCommunityRatings();
            return CreateRatingChart(items).BuildJson();
        }, "Calculate rating chart failed:");
    }

    private Task<string> CalculatePremiereYearChart()
    {
        return CalculateStat(async () =>
        {
            var yearDataList = await _showRepository.GetPremiereYears();
            return CalculatePremiereYearChart(yearDataList).BuildJson();
        }, "Calculate premiered year chart failed:");
    }

    private Task<string> CalculateOfficialRatingChart()
    {
        return CalculateStat(async () =>
        {
            var ratings = await _showRepository.GetOfficialRatingChartValues();
            return CalculateOfficialRatingChart(ratings).BuildJson();
        }, "Calculate official movie rating chart failed:");
    }
    
    private async Task<string> CalculateCollectedRateChart()
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
        }.BuildJson();
    }

    #endregion
    
    #region PieCharts
    
    private Task<string> CalculatePieChart(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.ShowStateChart => CalculateShowStateChart(),
            _ => Task.FromResult((string) null!)
        };
    }
    
    private Task<string> CalculateShowStateChart()
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

            }.BuildJson();
        }, "Calculate show state chart failed:");
    }
    
    #endregion

    #region ComplexCharts

    private Task<string> CalculateComplexChart(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.EpisodeWatchedPerHourOfDayChart => CalculateWatchedPerHourOfDayChart(),
            Statistic.EpisodeWatchedPerDayOfWeekChart => CalculateWatchedPerDayOfWeekChart(),
            _ => Task.FromResult((string) null!)
        };
    }
    
    private Task<string> CalculateWatchedPerDayOfWeekChart()
    {
        var wachtedPerDay = _showRepository.GetWatchedPerDayOfWeekChartValues;
        return GetWatchedPerDayOfWeekChart(wachtedPerDay, Constants.Shows.DaysOfTheWeek);
    }

    private Task<string> CalculateWatchedPerHourOfDayChart()
    {
        var watchedPerHour = _showRepository.GetWatchedPerHourOfDayChartValues;
        return GetWatchedPerHourOfDayChart(watchedPerHour, Constants.Shows.WatchedPerHour);
    }


    #endregion
}