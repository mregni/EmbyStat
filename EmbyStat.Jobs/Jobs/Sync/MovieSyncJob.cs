using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Genres.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.People.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;

namespace EmbyStat.Jobs.Jobs.Sync;

[DisableConcurrentExecution(60 * 60)]
public class MovieSyncJob : BaseJob, IMovieSyncJob
{
    private readonly IBaseHttpClient _baseHttpClient;
    private readonly IMovieRepository _movieRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMovieService _movieService;
    private readonly IGenreRepository _genreRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IFilterRepository _filterRepository;

    public MovieSyncJob(IHubHelper hubHelper, IJobRepository jobRepository,
        IConfigurationService configurationService, IClientStrategy clientStrategy,
        IMovieRepository movieRepository, IStatisticsRepository statisticsRepository, 
        IMovieService movieService, IGenreRepository genreRepository, IPersonRepository personRepository, 
        IMediaServerRepository mediaServerRepository, IFilterRepository filterRepository, ILogger<MovieSyncJob> logger) 
        : base(hubHelper, jobRepository, configurationService, logger)
    {
        _movieRepository = movieRepository;
        _statisticsRepository = statisticsRepository;
        _movieService = movieService;
        _genreRepository = genreRepository;
        _personRepository = personRepository;
        _mediaServerRepository = mediaServerRepository;
        _filterRepository = filterRepository;

        var settings = configurationService.Get();
        _baseHttpClient = clientStrategy.CreateHttpClient(settings.UserConfig.MediaServer.Type);
    }

    protected sealed override Guid Id => Constants.JobIds.MovieSyncId;
    protected override string JobPrefix => Constants.LogPrefix.MovieSyncJob;

    protected override async Task RunJobAsync()
    {
        if (!await IsMediaServerOnline())
        {
            var address = Configuration.UserConfig.MediaServer.Address;
            await LogWarning($"Halting task because we can't contact the server on {address}, please check the connection and try again.");
            return;
        }

        await ProcessGenresAsync();
        await LogProgress(7);
        await ProcessPeopleAsync();
        await LogProgress(15);

        await ProcessMoviesAsync();
        await LogProgress(75);

        await CalculateStatistics();
    }

    private async Task ProcessGenresAsync()
    {
        var genres = await _baseHttpClient.GetGenres();
        await LogInformation("Processing genres");
        await _genreRepository.UpsertRange(genres);
    }

    private async Task ProcessPeopleAsync()
    {
        var totalCount = await _baseHttpClient.GetPeopleCount();
        await LogInformation($"Fetching information from {totalCount} people");

        const int limit = 25000;
        var processed = 0;
        var j = 0;

        do
        {
            var people = await _baseHttpClient.GetPeople(j * limit, limit);
            await _personRepository.UpsertRange(people);

            processed += limit;
            j++;
        } while (processed < totalCount);
    }

    private async Task ProcessMoviesAsync()
    {
        var librariesToProcess = await _mediaServerRepository.GetAllSyncedLibraries(LibraryType.Movies);
        await LogInformation("Lets start processing movies");
        await LogInformation($"{librariesToProcess.Count} libraries are selected, getting ready for processing");

        var logIncrementBase = Math.Round(60 / (double)librariesToProcess.Count, 1);
        var genres = await _genreRepository.GetAll();

        foreach (var library in librariesToProcess)
        {
            await LogInformation($"Processing {library.Name}");
            await ProcessMovies(library, genres, logIncrementBase);
            await _mediaServerRepository.UpdateLibrarySyncDate(library.Id, DateTime.UtcNow, LibraryType.Movies);
            await LogInformation($"Processing {library.Name} done");
        }
    }

    private async Task ProcessMovies(Library library, IReadOnlyList<Genre> genres, double logIncrementBase)
    {
        var lastSynced = library.SyncTypes.GetLastSyncedDateForLibrary(library.Id, LibraryType.Movies);
        var totalCount = await _baseHttpClient.GetMediaCount(library.Id, lastSynced, "Movie");
        if (totalCount == 0)
        {
            await LogInformation("No changes detected to sync");
            return;
        }

        await LogInformation($"Found {totalCount} changed movies since last sync in {library.Name}");
        var processed = 0;
        var j = 0;
        const int limit = 50;
            
        var increment = logIncrementBase / (totalCount / (double)limit);
        do
        {
            var movies = await _baseHttpClient.GetMovies(library.Id, j * limit, limit, lastSynced);

            movies.AddGenres(genres);
            movies.ForEach(x => x.Library = library);
            await _movieRepository.UpsertRange(movies);

            processed += limit;
            j++;
            var logProcessed = processed < totalCount ? processed : totalCount;
            await LogInformation($"Processed { logProcessed } / { totalCount } movies");
            await LogProgressIncrement(increment);
        } while (processed < totalCount);
    }

    private async Task CalculateStatistics()
    {
        await LogInformation("Calculating movie statistics");
        await _statisticsRepository.MarkTypesAsInvalid(StatisticType.Movie);
        await LogProgress(77);
        await _movieService.CalculateMovieStatistics();
        await _filterRepository.DeleteAll(LibraryType.Movies);

        await LogInformation("Calculations done");
        await LogProgress(100);
    }

    #region Helpers

    private async Task<bool> IsMediaServerOnline()
    {
        _baseHttpClient.BaseUrl = Configuration.UserConfig.MediaServer.Address;
        return await _baseHttpClient.Ping();
    }

    #endregion
}