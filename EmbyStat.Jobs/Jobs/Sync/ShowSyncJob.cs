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
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MediaBrowser.Model.Net;

namespace EmbyStat.Jobs.Jobs.Sync
{
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

        public ShowSyncJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
            IClientStrategy clientStrategy, IShowRepository showRepository,
            IStatisticsRepository statisticsRepository, IShowService showService, ITmdbClient tmdbClient,
            IGenreRepository genreRepository, IPersonRepository personRepository, 
            IMediaServerRepository mediaServerRepository, IFilterRepository filterRepository)
            : base(hubHelper, jobRepository, settingsService, typeof(ShowSyncJob), Constants.LogPrefix.ShowSyncJob)
        {
            _showRepository = showRepository;
            _statisticsRepository = statisticsRepository;
            _showService = showService;
            _tmdbClient = tmdbClient;
            _genreRepository = genreRepository;
            _personRepository = personRepository;
            _mediaServerRepository = mediaServerRepository;
            _filterRepository = filterRepository;

            Title = jobRepository.GetById(Id).Title;
            var settings = settingsService.GetUserSettings();
            _baseHttpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.Type ?? ServerType.Emby);
        }

        public sealed override Guid Id => Constants.JobIds.ShowSyncId;
        public override string JobPrefix => Constants.LogPrefix.ShowSyncJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            if (!IsMediaServerOnline())
            {
                await LogWarning(
                    $"Halting task because we can't contact the server on {Settings.MediaServer.Address}, please check the connection and try again.");
                return;
            }

            await ProcessPeopleAsync();
            await LogProgress(7);
            await ProcessGenresAsync();
            await LogProgress(15);

            await ProcessShowsAsync();
            await LogProgress(75);

            await CalculateStatistics();
            await LogProgress(100);
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

                var increment = logIncrementBase / (totalCount / (double) 100);

                await LogInformation($"Found {totalCount} changed shows since last sync in {library.Name}");
                var processed = 0;
                var j = 0;
                const int limit = 50;
                do
                {
                    await ProcessShowBlock(library, genres, j, limit);

                    processed += limit;
                    j++;

                    var logProcessed = processed < totalCount ? processed : totalCount;
                    await LogInformation($"Processed {logProcessed} / {totalCount} shows");
                    await LogProgressIncrement(increment);
                } while (processed < totalCount);

                await _mediaServerRepository.UpdateLibrarySyncDate(library.Id, DateTime.UtcNow);
            }
        }

        private async Task ProcessShowBlock(Library library, IEnumerable<SqlGenre> genres, int index, int limit)
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
            }

            await _showRepository.UpsertShows(shows);
        }

        private async Task GetMissingEpisodesFromProviderAsync(SqlShow show)
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
                Logger.Error(e);
                show.ExternalSynced = false;
            }
        }

        private async Task ProcessMissingEpisodesAsync(SqlShow show)
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
                    Logger.Debug(
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
                    Logger.Debug($"Episode missing: {episode.Id} - {season.Id} - {show.Id} - {show.Name}");
                    show.Seasons.Single(x => x.Id == season.Id).Episodes.Add(episode);
                    missingEpisodesCount++;
                }
            }

            await LogInformation($"Found {missingEpisodesCount} missing episodes for show {show.Name}");

            show.ExternalSynced = true;
        }

        private static bool IsEpisodeMissing(IEnumerable<SqlEpisode> localEpisodes, SqlSeason season,
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

        private bool IsMediaServerOnline()
        {
            _baseHttpClient.BaseUrl = Settings.MediaServer.Address;
            return _baseHttpClient.Ping();
        }

        private async Task CalculateStatistics()
        {
            await LogInformation("Calculating show statistics");
            _statisticsRepository.MarkShowTypesAsInvalid();
            await _showService.CalculateShowStatistics();
            await _filterRepository.DeleteAll(LibraryType.TvShow);

            await LogInformation($"Calculations done");
            await LogProgress(100);
        }

        #endregion
    }
}