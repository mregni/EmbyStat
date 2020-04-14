using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Tvdb;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MediaBrowser.Model.Net;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60 * 60)]
    public class MediaSyncJob : BaseJob, IMediaSyncJob
    {
        private readonly IHttpClient _httpClient;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ITvdbClient _tvdbClient;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMovieService _movieService;
        private readonly IShowService _showService;

        public MediaSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
            IClientStrategy clientStrategy, IMovieRepository movieRepository, IShowRepository showRepository,
            ILibraryRepository libraryRepository, ITvdbClient tvdbClient, IStatisticsRepository statisticsRepository,
            IMovieService movieService, IShowService showService) : base(hubHelper, jobRepository, settingsService, 
            typeof(MediaSyncJob), Constants.LogPrefix.MediaSyncJob)
        {
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _libraryRepository = libraryRepository;
            _tvdbClient = tvdbClient;
            _statisticsRepository = statisticsRepository;
            _movieService = movieService;
            _showService = showService;
            Title = jobRepository.GetById(Id).Title;

            var settings = settingsService.GetUserSettings();
            _httpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.ServerType ?? ServerType.Emby);
        }

        public sealed override Guid Id => Constants.JobIds.MediaSyncId;
        public override string JobPrefix => Constants.LogPrefix.MediaSyncJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            var cancellationToken = new CancellationToken(false);

            if (!Settings.WizardFinished)
            {
                await LogWarning("Media sync task not running because wizard is not yet finished!");
                return;
            }

            if (!IsMediaServerOnline())
            {
                await LogWarning($"Halting task because we can't contact the server on {Settings.MediaServer.FullMediaServerAddress}, please check the connection and try again.");
                return;
            }

            await LogInformation("First delete all existing media and root media libraries from database so we have a clean start.");
            await LogProgress(3);

            var libraries = await GetLibrariesAsync();
            _libraryRepository.AddOrUpdateRange(libraries);
            await LogInformation($"Found {libraries.Count} root items, getting ready for processing");
            await LogProgress(15);

            await ProcessMoviesAsync(libraries, cancellationToken);
            await LogProgress(55);

            await ProcessShowsAsync(libraries, cancellationToken);
            await LogProgress(85);

            await CalculateStatistics();
            await LogProgress(100);
        }

        #region Movies
        private async Task ProcessMoviesAsync(IReadOnlyList<Library> libraries, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            _movieRepository.RemoveMovies();

            var neededLibraries = libraries.Where(x => Settings.MovieLibraryTypes.Any(y => y == x.Type)).ToList();
            for (var i = 0; i < neededLibraries.Count; i++)
            {
                var totalCount = await GetTotalLibraryMovieCount(neededLibraries[i].Id);
                if (totalCount == 0)
                {
                    continue;;
                }

                await LogInformation($"Found {totalCount} movies for {neededLibraries[i].Name} library");
                var processed = 0;
                var j = 0;
                const int limit = 100;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var movies = await PerformMovieSyncAsync(neededLibraries[i].Id, j * limit, limit);
                    _movieRepository.UpsertRange(movies);

                    processed += 100;
                    j++;
                    var logProcessed = processed < totalCount ? processed : totalCount;
                    await LogInformation($"Processed { logProcessed } / { totalCount } movies");
                } while (processed < totalCount);

                await LogProgress(Math.Round(15 + 40 * (i + 1) / (double)neededLibraries.Count, 1));
            }
        }

        private async Task<int> GetTotalLibraryMovieCount(string parentId)
        {
            var count = _httpClient.GetMovieCount(parentId);
            if (count == 0)
            {
               await LogWarning($"0 movies found in parent with id {parentId}. Propably means something is wrong with the HTTP call.");
            }

            return count;
        }

        private async Task<List<Movie>> PerformMovieSyncAsync(string parentId, int startIndex, int limit)
        {
            try
            {
                var movies = _httpClient.GetMovies(parentId, startIndex, limit);
                var boxSets = _httpClient.GetBoxSet(parentId);
                if (boxSets.Any())
                {
                    foreach (var parent in boxSets)
                    {
                        movies.AddRange(await PerformMovieSyncAsync(parent.Id, 0, 1000));
                    }
                }
                
                return movies;
            }
            catch (Exception e)
            {
                await LogError($"Movie error: {e.Message}");
                throw;
            }
        }

        #endregion

        #region Shows
        private async Task ProcessShowsAsync(IEnumerable<Library> libraries, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing show");
            var updateStartTime = DateTime.Now;

            await LogInformation("Logging in on the Tvdb API.");
            _tvdbClient.Login(Settings.Tvdb.ApiKey);
            var showsThatNeedAnUpdate = _tvdbClient.GetShowsToUpdate(Settings.Tvdb.LastUpdate);

            var neededLibraries = libraries.Where(x => Settings.ShowLibraryTypes.Any(y => y == x.Type)).ToList();
            for (var i = 0; i < neededLibraries.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await PerformShowSyncAsync(showsThatNeedAnUpdate, neededLibraries[i], updateStartTime);
                await LogProgress(Math.Round(55 + 30 * (i + 1) / (double)neededLibraries.Count, 1));
            }

            await LogInformation("Removing shows that are no longer present on your server (if any)");
            _showRepository.RemoveShowsThatAreNotUpdated(updateStartTime);
        }

        private async Task PerformShowSyncAsync(IReadOnlyList<string> showsThatNeedAnUpdate, Library library, DateTime updateStartTime)
        {
            var showList = _httpClient.GetShows(library.Id);

            var grouped = showList.GroupBy(x => x.TVDB).ToList();
            await LogInformation($"Found {grouped.Count} show for {library.Name} library");

            for (var i = 0; i < grouped.Count; i++)
            {
                var showGroup = grouped[i];

                var show = showGroup.First();
                foreach (var showDto in showGroup)
                {
                    var seasons = _httpClient.GetSeasons(showDto.Id);
                    var episodes = _httpClient.GetEpisodes(seasons.Select(x => x.Id), show.Id);

                    show.Seasons.AddRange(seasons.Where(x => show.Seasons.All(y => y.Id != x.Id)));
                    show.Episodes.AddRange(episodes.Where(x => show.Episodes.All(y => y.Id != x.Id)));
                }

                show.CumulativeRunTimeTicks = show.Episodes.Sum(x => x.RunTimeTicks ?? 0);
                show.LastUpdated = updateStartTime;

                var oldShow = _showRepository.GetShowById(show.Id);

                if (oldShow != null)
                {
                    show.TvdbFailed = oldShow.TvdbFailed;
                    show.TvdbSynced = oldShow.TvdbSynced;
                }

                if (!string.IsNullOrWhiteSpace(show.TVDB))
                {
                    if (!string.IsNullOrWhiteSpace(show.TVDB) &&
                        (show.HasShowChangedEpisodes(oldShow) || showsThatNeedAnUpdate.Any(x => x == show.Id)))
                    {
                        show = await GetMissingEpisodesFromTvdbAsync(show);
                    }
                    else if (oldShow != null && oldShow.NeedsShowSync())
                    {
                        show = await GetMissingEpisodesFromTvdbAsync(show);
                    }
                }

                _showRepository.InsertShow(show);
                await LogInformation($"Processed ({i + 1}/{grouped.Count}) {show.Name}");
            }
        }

        private async Task<Show> GetMissingEpisodesFromTvdbAsync(Show show)
        {
            try
            {
                return await ProcessMissingEpisodesAsync(show);
            }
            catch (HttpException e)
            {
                show.TvdbFailed = true;

                if (e.Message.Contains("404 Not Found"))
                {
                    await LogWarning($"Can't seem to find {show.Name} on Tvdb, skipping show for now");
                }
                return show;
            }
            catch (Exception e)
            {
                await LogError($"Can't seem to process show {show.Name}, check the logs for more details!");
                Logger.Error(e);
                show.TvdbFailed = true;
                return show;
            }
        }

        private async Task<Show> ProcessMissingEpisodesAsync(Show show)
        {
            var missingEpisodesCount = 0;
            var tvdbEpisodes = _tvdbClient.GetEpisodes(show.TVDB);

            foreach (var tvdbEpisode in tvdbEpisodes)
            {
                var season = show.Seasons.FirstOrDefault(x => x.IndexNumber == tvdbEpisode.SeasonNumber);
                
                if (season == null)
                {
                    Logger.Debug($"No season with index {tvdbEpisode.SeasonNumber} found for missing episode ({show.Name}), so we need to create one first");
                    season = tvdbEpisode.SeasonNumber.ConvertToSeason(show);
                    show.Seasons.Add(season);
                }

                if (IsEpisodeMissing(show.Episodes, season, tvdbEpisode))
                {
                    var episode = tvdbEpisode.ConvertToEpisode(show, season);
                    Logger.Debug($"Episode missing: { episode.Id } - {episode.ParentId} - {episode.ShowId} - {episode.ShowName}");
                    show.Episodes.Add(episode);
                    missingEpisodesCount++;
                }
            }

            await LogInformation($"Found {missingEpisodesCount} missing episodes for show {show.Name}");

            show.Episodes = show.Episodes.Where(x => show.Seasons.Any(y => y.Id.ToString() == x.ParentId)).ToList();
            show.TvdbSynced = true;
            return show;
        }

        private static bool IsEpisodeMissing(IEnumerable<Episode> localEpisodes, Season season, VirtualEpisode tvdbEpisode)
        {
            if (season == null)
            {
                return true;
            }

            foreach (var localEpisode in localEpisodes)
            {
                if (localEpisode.ParentId == season.Id)
                {
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
            }

            return true;
        }

        #endregion

        #region Helpers

        private bool IsMediaServerOnline()
        {
            _httpClient.BaseUrl = Settings.MediaServer.FullMediaServerAddress;
            return _httpClient.Ping();
        }

        private async Task CalculateStatistics()
        {
            await LogInformation("Calculating movie statistics");
            _statisticsRepository.MarkShowTypesAsInvalid();
            _statisticsRepository.MarkMovieTypesAsInvalid();

            var movieLibraries = _movieService.GetMovieLibraries().Select(x => x.Id).ToList();
            var movieLibrarySets = movieLibraries.PowerSets().ToList();
            for (var i = 0; i < movieLibrarySets.Count; i++)
            {
                _movieService.CalculateMovieStatistics(movieLibrarySets[i].ToList());
                await LogProgress(Math.Round(85 + 8 * (i + 1) / (double)movieLibrarySets.Count, 1));
            }

            await LogInformation("Calculating show statistics");
            var showLibraries = _showService.GetShowLibraries().Select(x => x.Id).ToList();
            var showLibrarySets = showLibraries.PowerSets().ToList();
            for (var i = 0; i < showLibrarySets.Count; i++)
            {
                var libraryList = showLibrarySets[i].ToList();
                _showService.CalculateShowStatistics(libraryList);
                _showService.CalculateCollectedRows(libraryList);

                await LogProgress(Math.Round(93 + 7 * (i + 1) / (double)showLibrarySets.Count, 1));
            }
        }

        private async Task<List<Library>> GetLibrariesAsync()
        {
            await LogInformation("Asking MediaServer for all root folders");
            var rootItems = _httpClient.GetMediaFolders();

            return rootItems.Items
                .Select(x => new Library
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.CollectionType.ToLibraryType(),
                    PrimaryImage = x.ImageTags.ContainsKey(ImageType.Primary) ? x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value : default(string)
                })
                .Where(x => x.Type != LibraryType.BoxSets)
                .ToList();
        }

        #endregion
    }
}
