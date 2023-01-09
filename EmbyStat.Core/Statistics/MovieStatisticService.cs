using System.Globalization;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.Extensions.Logging;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace EmbyStat.Core.Statistics;

public class MovieStatisticService : MediaStatisticService, IMovieStatisticsService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IConfigurationService _configurationService;

    public MovieStatisticService(IMovieRepository movieRepository, ILogger<MovieStatisticService> logger,
        IConfigurationService configurationService)
        : base(logger, configurationService)
    {
        _movieRepository = movieRepository;
        _configurationService = configurationService;
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
                return CalculateChart(uniqueType);
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
            Statistic.MovieTotalCount => CalculateTotalMovieCount(),
            Statistic.MovieTotalGenreCount => CalculateTotalMovieGenres(),
            Statistic.MovieTotalPlayLength => CalculateTotalPlayLength(),
            Statistic.MovieTotalDiskSpaceUsage => CalculateTotalDiskSpace(),
            Statistic.MovieTotalWatchedCount => CalculateTotalWatchedMovies(),
            Statistic.MovieTotalWatchedTime => CalculateTotalWatchedTime(),
            Statistic.MovieTotalActorCount => TotalPersonTypeCount(PersonType.Actor, Constants.Common.TotalActors),
            Statistic.MovieTotalDirectorCount => TotalPersonTypeCount(PersonType.Director,
                Constants.Common.TotalDirectors),
            Statistic.MovieTotalWriterCount => TotalPersonTypeCount(PersonType.Writer, Constants.Common.TotalWriters),
            Statistic.MovieTotalCurrentWatchingCount => CalculateTotalCurrentWatchingCount(),
            _ => Task.FromResult((string) null!)
        };
    }

    private Task<string> CalculateTotalMovieCount()
    {
        return CalculateStat(async () =>
        {
            var count = await _movieRepository.Count();
            return new Card
                {
                    Title = Constants.Movies.TotalMovies,
                    Value = count.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.TheatersRoundedIcon
                }
                .BuildJson();
        }, "Calculate total movie count failed:");
    }

    private Task<string> CalculateTotalMovieGenres()
    {
        return CalculateStat(async () =>
        {
            var totalGenres = await _movieRepository.GetGenreCount();
            return new Card
                {
                    Title = Constants.Common.TotalGenres,
                    Value = totalGenres.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.PoundRoundedIcon
                }
                .BuildJson();
        }, "Calculate total movie genres failed:");
    }

    private Task<string> CalculateTotalPlayLength()
    {
        return CalculateStat(async () =>
        {
            var playLengthTicks = await _movieRepository.GetTotalRuntime() ?? 0;
            var playLength = new TimeSpan(playLengthTicks);

            return new Card
                {
                    Title = Constants.Movies.TotalPlayLength,
                    Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                    Type = CardType.Time,
                    Icon = Constants.Icons.QueryBuilderRoundedIcon
                }
                .BuildJson();
        }, "Calculate total movie play length failed:");
    }

    private Task<string> CalculateTotalDiskSpace()
    {
        return CalculateStat(async () =>
        {
            var sum = await _movieRepository.GetTotalDiskSpace();
            return new Card
                {
                    Value = sum.ToString(CultureInfo.InvariantCulture),
                    Title = Constants.Common.TotalDiskSpace,
                    Type = CardType.Size,
                    Icon = Constants.Icons.StorageRoundedIcon
                }
                .BuildJson();
        }, "Calculate total movie disk space failed:");
    }

    private Task<string> CalculateTotalWatchedMovies()
    {
        return CalculateStat(async () =>
        {
            var viewCount = await _movieRepository.GetTotalWatchedMovieCount();
            return new Card
                {
                    Title = Constants.Movies.TotalWatchedMovies,
                    Value = viewCount.ToString(),
                    Type = CardType.Text,
                    Icon = Constants.Icons.PoundRoundedIcon
                }
                .BuildJson();
        }, "Calculate total watched movie count failed:");
    }

    private Task<string> CalculateTotalWatchedTime()
    {
        return CalculateStat(async () =>
        {
            var playLengthTicks = await _movieRepository.GetPlayedRuntime();
            var playLength = new TimeSpan(playLengthTicks);

            return new Card
                {
                    Title = Constants.Movies.PlayedPlayLength,
                    Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                    Type = CardType.Time,
                    Icon = Constants.Icons.QueryBuilderRoundedIcon
                }
                .BuildJson();
        }, "Calculate played movie play length failed:");
    }

    private Task<string> TotalPersonTypeCount(PersonType type, string title)
    {
        return CalculateStat(async () =>
        {
            var value = await _movieRepository.GetPeopleCount(type);
            return new Card
                {
                    Value = value.ToString(),
                    Title = title,
                    Icon = Constants.Icons.PeopleAltRoundedIcon,
                    Type = CardType.Text
                }
                .BuildJson();
        }, $"Calculate total {type} count failed:");
    }

    private Task<string> CalculateTotalCurrentWatchingCount()
    {
        var watchingValues = _movieRepository.GetCurrentWatchingCount;
        return GetCurrentWatchingCount(watchingValues, Constants.Movies.CurrentPlayingCount);
    }

    #endregion

    #region TopCards

    private Task<string> CalculateTopCard(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.MovieHighestRatedList => HighestRatedMovies(),
            Statistic.MovieLowestRatedList => LowestRatedMovies(),
            Statistic.MovieOldestPremieredList => OldestPremieredMovies(),
            Statistic.MovieNewestPremieredList => NewestPremieredMovies(),
            Statistic.MovieShortestList => ShortestMovies(),
            Statistic.MovieLongestList => LongestMovies(),
            Statistic.MovieLatestAddedList => LatestAddedMovies(),
            Statistic.MovieMostWatchedList => MostWatchedMovies(),
            _ => Task.FromResult((string) null!)
        };
    }

    private Task<string> HighestRatedMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetHighestRatedMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Movies.HighestRated, "/10", "CommunityRating", false)
                    .BuildJson()
                : null!;
        }, "Calculate highest rated movies failed:");
    }

    private Task<string> LowestRatedMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetLowestRatedMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Movies.LowestRated, "/10", "CommunityRating", false)
                    .BuildJson()
                : null!;
        }, "Calculate oldest premiered movies failed:");
    }

    private Task<string> OldestPremieredMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetOldestPremieredMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Movies.OldestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate oldest premiered movies failed:");
    }

    private Task<string> NewestPremieredMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetNewestPremieredMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list
                    .ConvertToTopCard(Constants.Movies.NewestPremiered, "COMMON.DATE", "PremiereDate", ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate newest premiered movies failed:");
    }

    private Task<string> ShortestMovies()
    {
        return CalculateStat<Task<string>>(async () =>
        {
            var config = _configurationService.Get();
            var toShortMovieTicks = TimeSpan.FromMinutes(config.UserConfig.ToShortMovie).Ticks;
            var list = await _movieRepository.GetShortestMovie(toShortMovieTicks, 5);
            return list.Count > 0
                ? list
                    .ConvertToTopCard(Constants.Movies.Shortest, "COMMON.MIN", "RunTimeTicks", ValueType.Ticks)
                    .BuildJson()
                : null!;
        }, "Calculate shortest movies failed:");
    }

    private Task<string> LongestMovies()
    {
        return CalculateStat(async () =>
        {
            var list = await _movieRepository.GetLongestMovie(5);
            return list.Count > 0
                ? list.ConvertToTopCard(Constants.Movies.Longest, "COMMON.MIN", "RunTimeTicks", ValueType.Ticks)
                    .BuildJson()
                : null!;
        }, "Calculate longest movies failed:");
    }

    private Task<string> LatestAddedMovies()
    {
        return CalculateStat(async () =>
        {
            var list = await _movieRepository.GetLatestAddedMovie(5);
            return list.Count > 0
                ? list.ConvertToTopCard(Constants.Movies.LatestAdded, "COMMON.DATE", "DateCreated",
                        ValueType.Date)
                    .BuildJson()
                : null!;
        }, "Calculate latest added movies failed:");
    }

    private Task<string> MostWatchedMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetMostWatchedMovies(5);

            return data.Count > 0
                ? data
                    .ConvertToTopCard(Constants.Movies.MostWatchedMovies, "#", false)
                    .BuildJson()
                : null!;
        }, "Calculate most watched shows failed:");
    }

    #endregion

    #region Charts

    private Task<string> CalculateChart(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.MovieGenreChart => CalculateGenreChart(),
            Statistic.MovieRatingChart => CalculateRatingChart(),
            Statistic.MoviePremiereYearChart => CalculatePremiereYearChart(),
            Statistic.MovieOfficialRatingChart => CalculateOfficialRatingChart(),
            _ => Task.FromResult((string) null!)
        };
    }

    private Task<string> CalculateGenreChart()
    {
        return CalculateStat(async () =>
        {
            var genres = await _movieRepository.GetGenreChartValues();
            return CreateGenreChart(genres).BuildJson();
        }, "Calculate genre chart failed:");
    }

    private Task<string> CalculateRatingChart()
    {
        return CalculateStat(async () =>
        {
            var items = await _movieRepository.GetCommunityRatings();
            return CreateRatingChart(items).BuildJson();
        }, "Calculate rating chart failed:");
    }

    private Task<string> CalculatePremiereYearChart()
    {
        return CalculateStat(async () =>
        {
            var yearDataList = await _movieRepository.GetPremiereYears();
            return CalculatePremiereYearChart(yearDataList).BuildJson();
        }, "Calculate premiered year chart failed:");
    }

    private Task<string> CalculateOfficialRatingChart()
    {
        return CalculateStat(async () =>
        {
            var ratings = await _movieRepository.GetOfficialRatingChartValues();
            return CalculateOfficialRatingChart(ratings).BuildJson();
        }, "Calculate official movie rating chart failed:");
    }

    #endregion

    #region ComplexCharts

    private Task<string> CalculateComplexChart(Statistic uniqueType)
    {
        return uniqueType switch
        {
            Statistic.MovieWatchedPerDayOfWeekChart => CalculateWatchedPerDayOfWeekChart(),
            Statistic.MovieWatchedPerHourOfDayChart => CalculateWatchedPerHourOfDayChart(),
            _ => Task.FromResult((string) null!)
        };
    }

    private Task<string> CalculateWatchedPerDayOfWeekChart()
    {
        var wachtedPerDay = _movieRepository.GetWatchedPerDayOfWeekChartValues;
        return GetWatchedPerDayOfWeekChart(wachtedPerDay, Constants.Movies.DaysOfTheWeek);
    }

    private Task<string> CalculateWatchedPerHourOfDayChart()
    {
        var watchedPerHour = _movieRepository.GetWatchedPerHourOfDayChartValues;
        return GetWatchedPerHourOfDayChart(watchedPerHour, Constants.Movies.WatchedPerHour);
    }

    #endregion
}