using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Clients.Tvdb;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60 * 60)]
    public class MediaSyncJob : BaseJob, IMediaSyncJob
    {
        private readonly IEmbyClient _embyClient;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ITvdbClient _tvdbClient;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMovieService _movieService;
        private readonly IShowService _showService;

        public MediaSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
            IEmbyClient embyClient, IMovieRepository movieRepository, IShowRepository showRepository,
            ILibraryRepository libraryRepository, ITvdbClient tvdbClient, IStatisticsRepository statisticsRepository,
            IMovieService movieService, IShowService showService) : base(hubHelper, jobRepository, settingsService)
        {
            _embyClient = embyClient;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _libraryRepository = libraryRepository;
            _tvdbClient = tvdbClient;
            _statisticsRepository = statisticsRepository;
            _movieService = movieService;
            _showService = showService;
            Title = jobRepository.GetById(Id).Title;
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

            if (!await IsEmbyAliveAsync())
            {
                await LogWarning($"Halting task because we can't contact the Emby server on {Settings.Emby.FullEmbyServerAddress}, please check the connection and try again.");
                return;
            }

            await LogInformation("First delete all existing media and root media libraries from database so we have a clean start.");
            await LogProgress(3);

            var libraries = await GetLibrariesAsync();
            _libraryRepository.AddOrUpdateRange(libraries);
            await LogInformation($"Found {libraries.Count} root items, getting ready for processing");
            await LogProgress(15);

            await ProcessMoviesAsync(libraries, cancellationToken);
            await LogProgress(35);

            await ProcessShowsAsync(libraries, cancellationToken);
            await LogProgress(55);

            await CalculateStatistics();
            await LogProgress(100);

            _embyClient.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
                await LogInformation($"Found {totalCount} movies for {neededLibraries[i].Name} library");
                var processed = 0;
                var j = 0;
                var limit = 100;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var movies = await PerformMovieSyncAsync(neededLibraries[i].Id, j * limit, limit);
                    _movieRepository.UpsertRange(movies);

                    processed += 100;
                    j++;
                    await LogInformation($"Processed { processed } / { totalCount } movies");
                    ;
                } while (processed < totalCount);

                await LogProgress(Math.Round(15 + 20 * (i + 1) / (double)neededLibraries.Count, 1));
            }
        }

        private async Task<int> GetTotalLibraryMovieCount(string parentId)
        {
            var countQuery = new ItemQuery
            {
                ParentId = parentId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { nameof(Movie), nameof(Boxset) },
                StartIndex = 0,
                Limit = 1,
                EnableTotalRecordCount = true
            };

            var totalCountResult = await _embyClient.GetItemsAsync(countQuery);
            return totalCountResult.TotalRecordCount;
        }

        private async Task<List<Movie>> PerformMovieSyncAsync(string parentId, int startIndex, int limit)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                LocationTypes = new[] { LocationType.FileSystem },
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { nameof(Movie), nameof(Boxset) },
                StartIndex = startIndex,
                Limit = limit,
                Fields = new[]
                    {
                        ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                        ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                        ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                        ItemFields.People, ItemFields.PremiereDate, ItemFields.CommunityRating, ItemFields.OfficialRating,
                        ItemFields.ProductionYear, ItemFields.RunTimeTicks
                    }
            };

            try
            {
                var embyMovies = await _embyClient.GetItemsAsync(query);
                var movies = embyMovies.Items
                    .Where(x => x.Type == Constants.Type.Movie)
                    .Select(x => MovieConverter.ConvertToMovie(x, parentId))
                    .ToList();

                if (embyMovies.Items.Any(x => x.Type == Constants.Type.Boxset))
                {
                    foreach (var parent in embyMovies.Items.Where(x => x.Type == Constants.Type.Boxset))
                    {
                        movies.AddRange(await PerformMovieSyncAsync(parent.Id, 0, 1000));
                    }
                }

                embyMovies.Items = new BaseItemDto[0];
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
            await _tvdbClient.Login(Settings.Tvdb.ApiKey, cancellationToken);
            var showsThatNeedAnUpdate = await _tvdbClient.GetShowsToUpdate(Settings.Tvdb.LastUpdate, cancellationToken);

            var neededLibraries = libraries.Where(x => Settings.ShowLibraryTypes.Any(y => y == x.Type)).ToList();
            for (var i = 0; i < neededLibraries.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await PerformShowSyncAsync(showsThatNeedAnUpdate, neededLibraries[i], updateStartTime);
                await LogProgress(Math.Round(35 + 20 * (i + 1) / (double)neededLibraries.Count, 1));
            }

            await LogInformation("Removing shows that are no longer present on your server (if any)");
            _showRepository.RemoveShowsThatAreNotUpdated(updateStartTime);
        }

        private async Task PerformShowSyncAsync(IReadOnlyList<int> showsThatNeedAnUpdate, Library library, DateTime updateStartTime)
        {
            var showList = await GetShowForLibraryAsync(library.Id);
            await LogInformation($"Found {showList.Items.Length} show for {library.Name} library");

            for (var i = 0; i < showList.TotalRecordCount; i++)
            {
                var showDto = showList.Items[i];
                var query = new ItemQuery
                {
                    EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                    ParentId = showDto.Id,
                    LocationTypes = new[] { LocationType.FileSystem },
                    Recursive = true,
                    UserId = Settings.Emby.UserId,
                    IncludeItemTypes = new[] { nameof(Season), nameof(Episode) },
                    Fields = new[]
                       {

                        ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                        ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                        ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.MediaSources,
                        ItemFields.MediaStreams, ItemFields.PremiereDate, ItemFields.CommunityRating,
                        ItemFields.OfficialRating, ItemFields.ProductionYear, ItemFields.Status, ItemFields.RunTimeTicks
                    }
                };

                var showChildren = await _embyClient.GetItemsAsync(query);

                var show = ShowConverter.ConvertToShow(showDto, library.Id);
                var seasons = showChildren.Items
                    .Where(x => x.ParentId == show.Id.ToString())
                    .Where(x => x.Type == nameof(Season))
                    .Select(ShowConverter.ConvertToSeason)
                    .ToList();

                var episodes = showChildren.Items
                    .Where(x => seasons.Any(y => y.Id.ToString() == x.ParentId))
                    .Where(x => x.Type == nameof(Episode))
                    .Select(x => ShowConverter.ConvertToEpisode(x, Convert.ToInt32(show.Id)))
                    .ToList();

                show.CumulativeRunTimeTicks = episodes.Sum(x => x.RunTimeTicks ?? 0);
                show.Seasons = seasons;
                show.Episodes = episodes;
                show.LastUpdated = updateStartTime;

                var oldShow = _showRepository.GetShowById(show.Id);

                if (oldShow != null)
                {
                    show.TvdbFailed = oldShow.TvdbFailed;
                    show.TvdbSynced = oldShow.TvdbSynced;
                }

                _showRepository.UpsertShow(show);
                if (!string.IsNullOrWhiteSpace(show.TVDB) && (show.HasShowChangedEpisodes(oldShow) || showsThatNeedAnUpdate.Any(x => x == show.Id)))
                {
                    await GetMissingEpisodesFromTvdbAsync(show, CancellationToken.None);
                }
                else if (oldShow != null && oldShow.NeedsShowSync())
                {
                    await GetMissingEpisodesFromTvdbAsync(show, CancellationToken.None);
                }

                await LogInformation($"Processed ({i}/{showList.TotalRecordCount}) {show.Name}");
            }
        }

        private Task<QueryResult<BaseItemDto>> GetShowForLibraryAsync(string libraryId)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = libraryId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                UserId = Settings.Emby.UserId,
                EnableTotalRecordCount = true,
                IncludeItemTypes = new[] { "Series" },
                Fields = new[] {
                    ItemFields.OriginalTitle, ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.PremiereDate,
                    ItemFields.CommunityRating, ItemFields.OfficialRating, ItemFields.ProductionYear,
                    ItemFields.Status, ItemFields.RunTimeTicks
                }
            };

            return _embyClient.GetItemsAsync(query);

        }

        private async Task GetMissingEpisodesFromTvdbAsync(Show show, CancellationToken cancellationToken)
        {
            try
            {
                await ProgressMissingEpisodesAsync(show, cancellationToken);
            }
            catch (HttpException e)
            {
                show.TvdbFailed = true;
                _showRepository.UpdateShow(show);

                if (e.Message.Contains("404 Not Found"))
                {
                    await LogWarning($"Can't seem to find {show.Name} on Tvdb, skipping show for now");
                }
            }
            catch (Exception e)
            {
                await LogError($"Can't seem to process show {show.Name}, check the logs for more details!");
                Logger.Error(e);
                show.TvdbFailed = true;
                _showRepository.UpdateShow(show);
            }
        }

        private async Task ProgressMissingEpisodesAsync(Show show, CancellationToken cancellationToken)
        {
            var missingEpisodesCount = 0;
            var tvdbEpisodes = await _tvdbClient.GetEpisodes(show.TVDB, cancellationToken);

            foreach (var tvdbEpisode in tvdbEpisodes)
            {
                var seasons = show.Seasons.Where(x => x.IndexNumber == tvdbEpisode.SeasonNumber).ToList();

                Season season;
                if (!seasons.Any())
                {
                    Logger.Debug($"No season with index {tvdbEpisode.SeasonNumber} found for missing episode ({show.Name}), so we need to create one first");
                    season = ShowConverter.ConvertToSeason(tvdbEpisode.SeasonNumber, show);
                    show.Seasons.Add(season);
                    _showRepository.AddSeason(season);
                }
                else
                {
                    season = seasons.First();
                }

                if (IsEpisodeMissing(show.Episodes, season, tvdbEpisode))
                {
                    var episode = tvdbEpisode.ConvertToEpisode(show, season);
                    _showRepository.AddEpisode(episode);
                    show.Episodes.Add(episode);
                    missingEpisodesCount++;
                }
            }

            await LogInformation($"Found {missingEpisodesCount} missing episodes for show {show.Name}");

            show.TvdbSynced = true;
            _showRepository.UpdateShow(show);
        }

        private static bool IsEpisodeMissing(IEnumerable<Episode> localEpisodes, Season season, VirtualEpisode tvdbEpisode)
        {
            if (season == null)
            {
                return true;
            }

            foreach (var localEpisode in localEpisodes)
            {
                if (localEpisode.ParentId == season.Id.ToString())
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

        private async Task<bool> IsEmbyAliveAsync()
        {
            var result = await _embyClient.PingEmbyAsync(Settings.Emby.FullEmbyServerAddress);
            return result == "Emby Server";
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
                await _movieService.CalculateMovieStatistics(movieLibrarySets[i].ToList());
                await LogProgress(Math.Round(85 + 8 * (i + 1) / (double)movieLibrarySets.Count, 1));
            }

            await LogInformation("Calculating show statistics");
            var showLibraries = _showService.GetShowLibraries().Select(x => x.Id).ToList();
            var showLibrarySets = showLibraries.PowerSets().ToList();
            for (var i = 0; i < showLibrarySets.Count; i++)
            {
                var libraryList = showLibrarySets[i].ToList();
                await _showService.CalculateShowStatistics(libraryList);
                _showService.CalculateCollectedRows(libraryList);

                await LogProgress(Math.Round(93 + 7 * (i + 1) / (double)showLibrarySets.Count, 1));
            }
        }

        private async Task<List<Library>> GetLibrariesAsync()
        {
            await LogInformation("Asking Emby for all root folders");
            var rootItems = await _embyClient.GetMediaFoldersAsync();

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

        public void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}
