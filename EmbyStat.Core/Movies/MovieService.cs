using System.Globalization;
using System.Text.Json;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Abstract;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace EmbyStat.Core.Movies;

public class MovieService : MediaService, IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IConfigurationService _configurationService;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMediaServerRepository _mediaServerRepository;

    public MovieService(IMovieRepository movieRepository,
        IConfigurationService configurationService, IStatisticsRepository statisticsRepository,
        IJobRepository jobRepository, IMediaServerRepository mediaServerRepository,
        ILogger<MovieService> logger) 
        : base(jobRepository, logger, configurationService)
    {
        _movieRepository = movieRepository;
        _configurationService = configurationService;
        _statisticsRepository = statisticsRepository;
        _mediaServerRepository = mediaServerRepository;
    }

    public Task<List<Library>> GetMovieLibraries()
    {
        return _mediaServerRepository.GetAllLibraries(LibraryType.Movies);
    }

    public async Task<MovieStatistics> GetStatistics()
    {
        var statistic = await _statisticsRepository.GetLastResultByType(StatisticType.Movie);

        MovieStatistics statistics;
        if (StatisticsAreValid(statistic, Constants.JobIds.MovieSyncId))
        {
            statistics = JsonConvert.DeserializeObject<MovieStatistics>(statistic.JsonResult) ?? new MovieStatistics();

            if (!_configurationService.Get().UserConfig.ToShortMovieEnabled
                && (statistics?.Shorts.Any() ?? false))
            {
                statistics.Shorts = new List<ShortMovie>(0);
            }
        }
        else
        {
            statistics = await CalculateMovieStatistics();
        }

        await AddLiveStatistics(statistics);

        return statistics;
    }

    private async Task AddLiveStatistics(MovieStatistics statistics)
    {
        statistics.ComplexCharts = await CalculateComplexCharts();
        statistics.Cards.AddIfNotNull(await GetCurrentWatchingCount(_movieRepository.GetCurrentWatchingCount, Constants.Movies.CurrentPlayingCount));
    }
    
    public async Task<MovieStatistics> CalculateMovieStatistics()
    {
        var statistics = new MovieStatistics();

        statistics.Cards = await CalculateCards();
        statistics.TopCards = await CalculateTopCards();
        statistics.Charts = await CalculateCharts();
        statistics.Shorts = CalculateShorts();
        statistics.NoImdb = CalculateNoImdbs();
        statistics.NoPrimary = CalculateNoPrimary();

        var json = JsonConvert.SerializeObject(statistics);
        await _statisticsRepository.ReplaceStatistic(json, DateTime.UtcNow, StatisticType.Movie);

        return statistics;
    }

    public bool TypeIsPresent()
    {
        return _movieRepository.Any();
    }

    public async Task<Page<Movie>> GetMoviePage(int skip, int take, string sortField, string sortOrder,
        Filter[] filters, bool requireTotalCount)
    {
        var list = await _movieRepository
            .GetMoviePage(skip, take, sortField, sortOrder, filters);

        var page = new Page<Movie>(list);
        if (requireTotalCount)
        {
            page.TotalCount = await _movieRepository.Count(filters);
        }

        return page;
    }

    public Task<Movie?> GetMovie(string id)
    {
        return _movieRepository.GetById(id);
    }

    public async Task SetLibraryAsSynced(string[] libraryIds)
    {
        await _mediaServerRepository.SetLibraryAsSynced(libraryIds, LibraryType.Movies);
        await _movieRepository.RemoveUnwantedMovies(libraryIds);
        await _statisticsRepository.MarkTypesAsInvalid(StatisticType.Movie);
    }

    #region Cards

    private async Task<List<Card>> CalculateCards()
    {
        var list = new List<Card>();
        list.AddIfNotNull(await CalculateTotalMovieCount());
        list.AddIfNotNull(await CalculateTotalMovieGenres());
        list.AddIfNotNull(CalculateTotalPlayLength());
        list.AddIfNotNull(CalculateTotalDiskSpace());
        list.AddIfNotNull(CalculateTotalWatchedMovies());
        list.AddIfNotNull(await CalculateTotalWatchedTime());
        list.AddIfNotNull(TotalPersonTypeCount(PersonType.Actor, Constants.Common.TotalActors));
        list.AddIfNotNull(TotalPersonTypeCount(PersonType.Director, Constants.Common.TotalDirectors));
        list.AddIfNotNull(TotalPersonTypeCount(PersonType.Writer, Constants.Common.TotalWriters));

        return list;
    }

    private Task<Card> CalculateTotalMovieCount()
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
            };
        }, "Calculate total movie count failed:");
    }

    private Task<Card> CalculateTotalMovieGenres()
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
            };
        }, "Calculate total movie genres failed:");
    }

    private Card CalculateTotalPlayLength()
    {
        return CalculateStat(() =>
        {
            var playLengthTicks = _movieRepository.GetTotalRuntime() ?? 0;
            var playLength = new TimeSpan(playLengthTicks);

            return new Card
            {
                Title = Constants.Movies.TotalPlayLength,
                Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                Type = CardType.Time,
                Icon = Constants.Icons.QueryBuilderRoundedIcon
            };
        }, "Calculate total movie play length failed:");
    }

    private Card CalculateTotalDiskSpace()
    {
        return CalculateStat(() =>
        {
            var sum = _movieRepository.GetTotalDiskSpace();
            return new Card
            {
                Value = sum.ToString(CultureInfo.InvariantCulture),
                Title = Constants.Common.TotalDiskSpace,
                Type = CardType.Size,
                Icon = Constants.Icons.StorageRoundedIcon
            };
        }, "Calculate total movie disk space failed:");
    }

    private Card CalculateTotalWatchedMovies()
    {
        return CalculateStat(() =>
        {
            var viewCount = _movieRepository.GetTotalWatchedMovieCount();
            return new Card
            {
                Title = Constants.Movies.TotalWatchedMovies,
                Value = viewCount.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }, "Calculate total watched movie count failed:");
    }
    
    private Task<Card> CalculateTotalWatchedTime()
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
            };
        }, "Calculate played movie play length failed:");
    }

    private Card TotalPersonTypeCount(PersonType type, string title)
    {
        return CalculateStat(() =>
        {
            var value = _movieRepository.GetPeopleCount(type);
            return new Card
            {
                Value = value.ToString(),
                Title = title,
                Icon = Constants.Icons.PeopleAltRoundedIcon,
                Type = CardType.Text
            };
        }, $"Calculate total {type} count failed:");
    }

    #endregion

    #region TopCards

    private async Task<List<TopCard>> CalculateTopCards()
    {
        var list = new List<TopCard>();
        list.AddIfNotNull(await HighestRatedMovie());
        list.AddIfNotNull(await LowestRatedMovie());
        list.AddIfNotNull(await OldestPremieredMovie());
        list.AddIfNotNull(await NewestPremieredMovie());
        list.AddIfNotNull(ShortestMovie());
        list.AddIfNotNull(LongestMovie());
        list.AddIfNotNull(LatestAddedMovie());
        list.AddIfNotNull(await MostWatchedMovies());
        return list;
    }

    private Task<TopCard?> HighestRatedMovie()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetHighestRatedMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.HighestRated, "/10", "CommunityRating", false)
                : null;
        }, "Calculate highest rated movies failed:");
    }

    private Task<TopCard?> LowestRatedMovie()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetLowestRatedMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.LowestRated, "/10", "CommunityRating", false)
                : null;
        }, "Calculate oldest premiered movies failed:");
    }

    private Task<TopCard?> OldestPremieredMovie()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetOldestPremieredMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.OldestPremiered, "COMMON.DATE", "PremiereDate",
                    ValueType.Date)
                : null;
        }, "Calculate oldest premiered movies failed:");
    }

    private Task<TopCard?> NewestPremieredMovie()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetNewestPremieredMedia(5);
            var list = data.ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.NewestPremiered, "COMMON.DATE", "PremiereDate",
                    ValueType.Date)
                : null;
        }, "Calculate newest premiered movies failed:");
    }

    private TopCard? ShortestMovie()
    {
        return CalculateStat(() =>
        {
            var config = _configurationService.Get();
            var toShortMovieTicks = TimeSpan.FromMinutes(config.UserConfig.ToShortMovie).Ticks;
            var list = _movieRepository.GetShortestMovie(toShortMovieTicks, 5).ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.Shortest, "COMMON.MIN", "RunTimeTicks",
                    ValueType.Ticks)
                : null;
        }, "Calculate shortest movies failed:");
    }

    private TopCard? LongestMovie()
    {
        return CalculateStat(() =>
        {
            var list = _movieRepository.GetLongestMovie(5).ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.Longest, "COMMON.MIN", "RunTimeTicks", ValueType.Ticks)
                : null;
        }, "Calculate longest movies failed:");
    }

    private TopCard? LatestAddedMovie()
    {
        return CalculateStat(() =>
        {
            var list = _movieRepository.GetLatestAddedMedia(5).ToArray();
            return list.Length > 0
                ? list.ConvertToTopCard(Constants.Movies.LatestAdded, "COMMON.DATE", "DateCreated",
                    ValueType.Date)
                : null;
        }, "Calculate latest added movies failed:");
    }
    
    private Task<TopCard?> MostWatchedMovies()
    {
        return CalculateStat(async () =>
        {
            var data = await _movieRepository.GetMostWatchedMovies(5);

            return data.Count > 0
                ? data.ConvertToTopCard(Constants.Movies.MostWatchedMovies, "#", false)
                : null;
        }, "Calculate most watched shows failed:");
    }

    #endregion

    #region Charts

    private async Task<List<Chart>> CalculateCharts()
    {
        var list = new List<Chart>();
        list.AddIfNotNull(await CalculateGenreChart());
        list.AddIfNotNull(CalculateRatingChart());
        list.AddIfNotNull(CalculatePremiereYearChart());
        list.AddIfNotNull(await CalculateOfficialRatingChart());

        return list;
    }

    private Task<Chart> CalculateGenreChart()
    {
        return CalculateStat(async () =>
        {
            var genres = await _movieRepository.GetGenreChartValues();
            return CreateGenreChart(genres);
        }, "Calculate genre chart failed:");
    }

    private Chart CalculateRatingChart()
    {
        return CalculateStat(() =>
        {
            var items = _movieRepository.GetCommunityRatings();
            return CreateRatingChart(items);
        }, "Calculate rating chart failed:");
    }

    private Chart CalculatePremiereYearChart()
    {
        return CalculateStat(() =>
        {
            var yearDataList = _movieRepository.GetPremiereYears();
            return CalculatePremiereYearChart(yearDataList);
        }, "Calculate premiered year chart failed:");
    }

    private Task<Chart> CalculateOfficialRatingChart()
    {
        return CalculateStat(async () =>
        {
            var ratings = await _movieRepository.GetOfficialRatingChartValues();
            return CalculateOfficialRatingChart(ratings);
        }, "Calculate official movie rating chart failed:");
    }
    
    #endregion

    #region ComplexCharts

    private async Task<List<MultiChart>> CalculateComplexCharts()
    {
        var list = new List<MultiChart>();
        var watchedPerHour = _movieRepository.GetWatchedPerHourOfDayChartValues;
        list.AddIfNotNull(await CalculateWatchedPerHourOfDayChart(watchedPerHour, Constants.Movies.WatchedPerHour));

        var wachtedPerDay = _movieRepository.GetWatchedPerDayOfWeekChartValues;
        list.AddIfNotNull(await CalculateWatchedPerDayOfWeekChart(wachtedPerDay, Constants.Movies.DaysOfTheWeek));

        return list;
    }

    #endregion

    #region Suspicious

    private IEnumerable<ShortMovie> CalculateShorts()
    {
        var config = _configurationService.Get();
        if (!config.UserConfig.ToShortMovieEnabled)
        {
            return new List<ShortMovie>(0);
        }

        var shortMovies = _movieRepository.GetToShortMovieList(config.UserConfig.ToShortMovie);
        return shortMovies.Select((t, i) => new ShortMovie
        {
            Number = i,
            Duration = Math.Floor(new TimeSpan(t.RunTimeTicks ?? 0).TotalMinutes),
            Title = t.Name,
            MediaId = t.Id
        }).ToList();
    }

    private IEnumerable<SuspiciousMovie> CalculateNoImdbs()
    {
        var moviesWithoutImdbId = _movieRepository.GetMoviesWithoutImdbId();
        return moviesWithoutImdbId
            .Select((t, i) => new SuspiciousMovie
            {
                Number = i,
                Title = t.Name,
                MediaId = t.Id
            });
    }

    private IEnumerable<SuspiciousMovie> CalculateNoPrimary()
    {
        var noPrimaryImageMovies = _movieRepository.GetMoviesWithoutPrimaryImage();
        return noPrimaryImageMovies.Select((t, i) => new SuspiciousMovie
            {
                Number = i,
                Title = t.Name,
                MediaId = t.Id
            })
            .ToList();
    }

    #endregion
}