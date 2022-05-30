using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Tmdb;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Show;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Genres.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.People.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;
using MediaBrowser.Model.Net;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Sync;

[DisableConcurrentExecution(60 * 60)]
public class ShowSyncJob : BaseJob, IShowSyncJob
{
    private readonly IBaseHttpClient _baseHttpClient;
    private readonly IShowRepository _showRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IShowService _showService;
    private readonly ITmdbClient _tmdbClient;
    private readonly IGenreRepository _genreRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IFilterRepository _filterRepository;

    public ShowSyncJob(IHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService,
        IClientStrategy clientStrategy, IShowRepository showRepository,
        IStatisticsRepository statisticsRepository, IShowService showService, ITmdbClient tmdbClient,
        IGenreRepository genreRepository, IPersonRepository personRepository, 
        IMediaServerRepository mediaServerRepository, IFilterRepository filterRepository, ILogger<ShowSyncJob> logger)
        : base(hubHelper, jobRepository, configurationService, logger)
    {
        _showRepository = showRepository;
        _statisticsRepository = statisticsRepository;
        _showService = showService;
        _tmdbClient = tmdbClient;
        _genreRepository = genreRepository;
        _personRepository = personRepository;
        _mediaServerRepository = mediaServerRepository;
        _filterRepository = filterRepository;

        var settings = configurationService.Get();
        _baseHttpClient = clientStrategy.CreateHttpClient(settings.UserConfig.MediaServer.Type);
    }

    protected sealed override Guid Id => Constants.JobIds.ShowSyncId;
    protected override string JobPrefix => Constants.LogPrefix.ShowSyncJob;

    protected override async Task RunJobAsync()
    {
        if (!await IsMediaServerOnline())
        {
            var address = Configuration.UserConfig.MediaServer.Address;
            await LogWarning($"Halting task because we can't contact the server on {address}, please check the connection and try again.");
            return;
        }

        await ProcessPeopleAsync();
        await LogProgress(7);
        await ProcessGenresAsync();
        await LogProgress(15);

        await ProcessShowsAsync();
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
        await LogInformation($"Processing information from {totalCount} people");

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

    private async Task ProcessShowsAsync()
    {
        var librariesToProcess = await _mediaServerRepository.GetAllLibraries(LibraryType.TvShow, true);
        await LogInformation("Processing shows");
        await LogInformation(
            $"{librariesToProcess.Count} libraries are selected, getting ready for processing");

        var logIncrementBase = Math.Round(60 / (double) librariesToProcess.Count, 1);
        var genres = await _genreRepository.GetAll();

        foreach (var library in librariesToProcess)
        {
            var totalCount = await _baseHttpClient.GetMediaCount(library.Id, library.LastSynced, "Series");
            if (totalCount == 0)
            {
                continue;
            }

            var increment = logIncrementBase / totalCount;

            await LogInformation($"Found {totalCount} changed shows since last sync in {library.Name}");
            var processed = 0;
            var j = 0;
            const int limit = 50;
            do
            {
                await ProcessShowBlock(library, genres, j, limit, increment);

                processed += limit;
                j++;

                var logProcessed = processed < totalCount ? processed : totalCount;
                await LogInformation($"Processed {logProcessed} / {totalCount} shows");
            } while (processed < totalCount);

            await _mediaServerRepository.UpdateLibrarySyncDate(library.Id, DateTime.UtcNow);
        }
    }

    private async Task ProcessShowBlock(Library library, IEnumerable<Genre> genres, int index, int limit, double increment)
    {
        var shows = await _baseHttpClient.GetShows(library.Id, index * limit, limit, library.LastSynced);
        shows.AddGenres(genres);

        foreach (var show in shows)
        {
            var seasons = await _baseHttpClient.GetSeasons(show.Id, library.LastSynced);
            var episodes = await _baseHttpClient.GetEpisodes(show.Id, library.LastSynced);

            foreach (var season in seasons)
            {
                season.Episodes.AddRange(episodes.Where(x => x.SeasonId == season.Id));
            }

            show.Seasons = seasons.ToList();
            show.CumulativeRunTimeTicks = show.GetShowRunTimeTicks();
            show.SizeInMb = show.GetShowSize();

            var localShow = await _showRepository.GetShowByIdWithEpisodes(show.Id);
            if (!show.TMDB.HasValue)
            {
                continue;
            }

            if (show.HasShowChangedEpisodes(localShow) || localShow is {ExternalSynced: false})
            {
                await GetMissingEpisodesFromProviderAsync(show);
            }
            
            await LogProgressIncrement(increment);
        }

        await _showRepository.UpsertShows(shows);
    }

    private async Task GetMissingEpisodesFromProviderAsync(Show show)
    {
        try
        {
            await ProcessMissingEpisodesAsync(show);
        }
        catch (HttpException e)
        {
            show.ExternalSynced = false;

            if (e.Message.Contains("404 Not Found"))
            {
                await LogWarning($"Can't seem to find {show.Name} on Imdb, skipping show for now");
            }
        }
        catch (Exception e)
        {
            await LogError($"Can't seem to process show {show.Name}, check the logs for more details!");
            Logger.LogError(e, "Exception details");
            show.ExternalSynced = false;
        }
    }

    private async Task ProcessMissingEpisodesAsync(Show show)
    {
        var missingEpisodesCount = 0;
        var externalEpisodes = await _tmdbClient.GetEpisodesAsync(show.TMDB);

        if (externalEpisodes == null)
        {
            throw new NotFoundException($"Could not find show {show.Name} with id {show.TMDB}");
        }

        foreach (var externalEpisode in externalEpisodes.Where(x => x.SeasonNumber != 0))
        {
            var season = show.Seasons.FirstOrDefault(x => x.IndexNumber == externalEpisode.SeasonNumber);
            if (season == null)
            {
                Logger.LogDebug(
                    $"No season with index {externalEpisode.SeasonNumber} found for missing episode ({show.Name}), so we need to create one first");
                season = externalEpisode.SeasonNumber.ConvertToVirtualSeason(show);
                show.Seasons.Add(season);
            }

            var localEpisodes = show.Seasons
                .Where(x => x.Episodes != null)
                .SelectMany(x => x.Episodes);
            if (IsEpisodeMissing(localEpisodes, season, externalEpisode))
            {
                var episode = externalEpisode.ConvertToVirtualEpisode(season);
                Logger.LogDebug($"Episode missing: {episode.Id} - {season.Id} - {show.Id} - {show.Name}");
                show.Seasons.Single(x => x.Id == season.Id).Episodes.Add(episode);
                missingEpisodesCount++;
            }
        }

        await LogInformation($"Found {missingEpisodesCount} missing episodes for show {show.Name}");

        show.ExternalSynced = true;
    }

    private static bool IsEpisodeMissing(IEnumerable<Episode> localEpisodes, Season season,
        VirtualEpisode tvdbEpisode)
    {
        if (season == null || localEpisodes == null)
        {
            return true;
        }

        foreach (var localEpisode in localEpisodes)
        {
            if (localEpisode.SeasonId != season.Id) continue;
            if (!localEpisode.IndexNumberEnd.HasValue)
            {
                if (localEpisode.IndexNumber == tvdbEpisode.EpisodeNumber)
                {
                    return false;
                }
            }
            else
            {
                if (localEpisode.IndexNumber <= tvdbEpisode.EpisodeNumber &&
                    localEpisode.IndexNumberEnd >= tvdbEpisode.EpisodeNumber)
                {
                    return false;
                }
            }
        }

        return true;
    }

    #region Helpers

    private async Task<bool> IsMediaServerOnline()
    {
        _baseHttpClient.BaseUrl = Configuration.UserConfig.MediaServer.Address;
        return await _baseHttpClient.Ping();
    }

    private async Task CalculateStatistics()
    {
        await LogInformation("Calculating show statistics");
        await _statisticsRepository.MarkTypesAsInvalid(StatisticType.Show);
        await LogProgress(75);

        await _showService.CalculateShowStatistics();
        await _filterRepository.DeleteAll(LibraryType.TvShow);

        await LogInformation("Calculations done");
        await LogProgress(100);
    }

    #endregion
}