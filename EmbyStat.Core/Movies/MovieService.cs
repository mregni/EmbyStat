using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Newtonsoft.Json;

namespace EmbyStat.Core.Movies;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IConfigurationService _configurationService;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IStatisticsService _statisticsService;

    public MovieService(IMovieRepository movieRepository,
        IConfigurationService configurationService, IMediaServerRepository mediaServerRepository,
        IStatisticsService statisticsService)
    {
        _movieRepository = movieRepository;
        _configurationService = configurationService;
        _mediaServerRepository = mediaServerRepository;
        _statisticsService = statisticsService;
    }

    public Task<List<Library>> GetMovieLibraries()
    {
        return _mediaServerRepository.GetAllLibraries(LibraryType.Movies);
    }

    public async Task<MovieStatistics> GetStatistics()
    {
        var page = await _statisticsService.GetPage(Constants.StatisticPageIds.MoviePage) ??
                   await _statisticsService.CalculatePage(Constants.StatisticPageIds.MoviePage);

        if (page != null)
        {
            var currentWatchingCard = page.PageCards
                .SingleOrDefault(x => x.StatisticCard.UniqueType == Statistic.MovieTotalCurrentWatchingCount);
            if (currentWatchingCard != null)
            {
                await _statisticsService.CalculateCard(currentWatchingCard.StatisticCard);
            }
            return new MovieStatistics(page);
        }

        throw new NotFoundException($"Page {Constants.StatisticPageIds.MoviePage} is not found");
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
    }


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