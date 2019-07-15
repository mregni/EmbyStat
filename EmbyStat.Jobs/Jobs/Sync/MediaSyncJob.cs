using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using MediaBrowser.Model.Querying;
using NLog;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60 * 60)]
    public class MediaSyncJob : BaseJob, IMediaSyncJob
    {
        private readonly IEmbyClient _embyClient;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITvdbClient _tvdbClient;
        private readonly IStatisticsRepository _statisticsRepository;

        public MediaSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
            IEmbyClient embyClient, IMovieRepository movieRepository, IShowRepository showRepository,
            IPersonRepository personRepository, ICollectionRepository collectionRepository, ITvdbClient tvdbClient,
            IStatisticsRepository statisticsRepository) : base(hubHelper, jobRepository, settingsService)
        {
            _embyClient = embyClient;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _personRepository = personRepository;
            _collectionRepository = collectionRepository;
            _tvdbClient = tvdbClient;
            _statisticsRepository = statisticsRepository;
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

            if (!await IsEmbyAliveAsync(cancellationToken))
            {
                await LogWarning($"Halting task because we can't contact the Emby server on {Settings.FullEmbyServerAddress}, please check the connection and try again.");
                return;
            }

            await LogInformation("First delete all existing media and root media collections from database so we have a clean start.");
            var oldShows = _showRepository.GetAllShows(Array.Empty<string>(), false, true).ToList();
            CleanUpDatabase();
            await LogProgress(3);

            var collections = await GetCollectionsAsync(cancellationToken);
            _collectionRepository.AddOrUpdateRange(collections);
            await LogInformation($"Found {collections.Count} root items, getting ready for processing");

            await ProcessPeopleAsync(cancellationToken);
            await LogProgress(15);

            await ProcessMoviesAsync(collections, cancellationToken);
            await LogProgress(50);

            await ProcessShowsAsync(collections, oldShows, cancellationToken);
            await LogProgress(85);

            await SyncMissingEpisodesAsync(oldShows, cancellationToken);

            _statisticsRepository.MarkShowTypesAsInvalid();
            _statisticsRepository.MarkMovieTypesAsInvalid();
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _showRepository.RemoveShows();
        }

        private async Task<bool> IsEmbyAliveAsync(CancellationToken cancellationToken)
        {
            var result = await _embyClient.PingEmbyAsync(Settings.FullEmbyServerAddress, cancellationToken);
            return result == "Emby Server";
        }

        private async Task ProcessPeopleAsync(CancellationToken cancellationToken)
        {
            var embyPeople = await _embyClient.GetPeopleAsync(new PersonsQuery(), cancellationToken);
            await LogInformation($"Need to add/update {embyPeople.TotalRecordCount} people first.");

            var people = embyPeople.Items.DistinctBy(x => x.Id).Select(PersonConverter.ConvertToSmallPerson);
            _personRepository.UpserRange(people);
        }

        #region Movies
        private async Task ProcessMoviesAsync(IReadOnlyList<Collection> collections, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            await LogProgress(5);

            var neededCollections = collections.Where(x => Settings.MovieCollectionTypes.Any(y => y == x.Type)).ToList();

            foreach (var collection in neededCollections)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var movies = await PerformMovieSyncAsync(collection.Id);
                _movieRepository.UpsertRange(movies);
                await LogProgress(15 + Math.Round(35 / (double)neededCollections.Count, 1));
            }
        }

        private async Task<IEnumerable<Movie>> PerformMovieSyncAsync(string collectionId)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = collectionId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { nameof(Movie) },
                Fields = new[]
                    {
                        ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                        ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                        ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                        ItemFields.People
                    }
            };

            try
            {
                var embyMovies = await _embyClient.GetItemsAsync(query, new CancellationToken(false));
                var movies = embyMovies.Items.Where(x => x.Type == Constants.Type.Movie).Select(x => MovieConverter.ConvertToMovie(x, collectionId)).ToList();

                var recursiveMovies = new List<Movie>();
                if (embyMovies.Items.Any(x => x.Type == Constants.Type.Boxset))
                {
                    foreach (var parent in embyMovies.Items.Where(x => x.Type == Constants.Type.Boxset))
                    {
                        recursiveMovies.AddRange(await PerformMovieSyncAsync(parent.Id));
                    }
                }

                movies.AddRange(recursiveMovies);
                return movies.DistinctBy(x => x.Id);
            }
            catch (Exception e)
            {
                await LogError($"Movie error: {e.Message}");
                throw;
            }
        }

        #endregion

        #region Shows
        private async Task ProcessShowsAsync(List<Collection> collections, IReadOnlyList<Show> oldSHows, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing shows");
            await LogProgress(33);

            var neededCollections = collections.Where(x => Settings.ShowCollectionTypes.Any(y => y == x.Type)).ToList();

            foreach (var collection in neededCollections)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await PerformShowSyncAsync(oldSHows, collection.Id);
                await LogProgress(50 + Math.Round(35 / (double) neededCollections.Count, 1));
            }
        }

        private async Task PerformShowSyncAsync(IReadOnlyList<Show> oldShows, string collectionId)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = collectionId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { "Series", nameof(Season), nameof(Episode) },
                Fields = new[]
                {

                    ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People,
                    ItemFields.MediaSources, ItemFields.MediaStreams
                }
            };
            var embyShows = await _embyClient.GetItemsAsync(query);

            var shows = embyShows.Items
                .Where(x => x.Type == "Series")
                .Select(x => ShowConverter.ConvertToShow(x, collectionId))
                .ToList();

            Parallel.ForEach(shows, show =>
            {
                var seasons = embyShows.Items
                    .Where(x => x.ParentId == show.Id.ToString())
                    .Where(x => x.Type == nameof(Season))
                    .Select(ShowConverter.ConvertToSeason)
                    .ToList();

                var episodes = embyShows.Items
                    .Where(x => seasons.Any(y => y.Id.ToString() == x.ParentId))
                    .Where(x => x.Type == nameof(Episode))
                    .Select(x => ShowConverter.ConvertToEpisode(x, show))
                    .ToList();

                show.Seasons = seasons;
                show.Episodes = episodes;

                var oldShow = oldShows.SingleOrDefault(x => x.Id == show.Id);
                if (oldShow != null)
                {
                    show.TvdbFailed = oldShow.TvdbFailed;
                    show.TvdbSynced = oldShow.TvdbSynced;
                    show.MissingEpisodesCount = oldShow.MissingEpisodesCount;
                }

                _showRepository.InsertSeasonsBulk(seasons);
                _showRepository.InsertEpisodesBulk(episodes);
            });

            _showRepository.InsertShowsBulk(shows);
        }

        private async Task SyncMissingEpisodesAsync(IReadOnlyList<Show> oldShows, CancellationToken cancellationToken)
        {
            await LogInformation("Started checking missing episodes");
            await _tvdbClient.Login(Settings.Tvdb.ApiKey, cancellationToken);

            var shows = _showRepository.GetAllShowsWithTvdbId().ToList();
            var showsWithMissingEpisodes = shows.GetNeverSyncedShows().ToList();
            showsWithMissingEpisodes.AddRange(shows.GetShowsWithChangedEpisodes(oldShows));
            
            if (Settings.Tvdb.LastUpdate.HasValue)
            {
                var showsThatNeedAnUpdate = await _tvdbClient.GetShowsToUpdate(shows.Select(x => x.TVDB),
                    Settings.Tvdb.LastUpdate.Value, cancellationToken);
                showsWithMissingEpisodes.AddRange(shows.Where(x => showsThatNeedAnUpdate.Any(y => y == x.TVDB)));
            }

            showsWithMissingEpisodes = showsWithMissingEpisodes.DistinctBy(x => x.TVDB).ToList();

            var now = DateTime.Now;
            await GetMissingEpisodesFromTvdbAsync(showsWithMissingEpisodes, cancellationToken);

            Settings.Tvdb.LastUpdate = now;
            await SettingsService.SaveUserSettingsAsync(Settings);
        }

        private async Task GetMissingEpisodesFromTvdbAsync(List<Show> shows, CancellationToken cancellationToken)
        {
            foreach (var show in shows)
            {
                try
                {
                    await ProgressMissingEpisodesAsync(show, cancellationToken);
                    await LogProgress(85 + Math.Round(15 / (double)shows.Count, 1));
                }
                catch (Exception e)
                {
                    show.TvdbFailed = true;
                    _showRepository.UpdateShow(show);
                }
            }
        }

        private async Task ProgressMissingEpisodesAsync(Show show, CancellationToken cancellationToken)
        {
            var neededEpisodeCount = 0;

            var tvdbEpisodes = await _tvdbClient.GetEpisodes(show.TVDB, cancellationToken);

            foreach (var episode in tvdbEpisodes)
            {
                var season = show.Seasons.SingleOrDefault(x => x.IndexNumber == episode.SeasonIndex);
                if (IsEpisodeMissing(show.Episodes, season, episode))
                {
                    neededEpisodeCount++;
                }
            }

            await LogInformation($"Found {neededEpisodeCount} missing episodes for show {show.Name}");
            show.TvdbSynced = true;
            show.MissingEpisodesCount = neededEpisodeCount;
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

                        if (localEpisode.IndexNumber == tvdbEpisode.EpisodeIndex)
                        {
                            return false;
                        }

                    }
                    else
                    {

                        if (localEpisode.IndexNumber <= tvdbEpisode.EpisodeIndex &&
                            localEpisode.IndexNumberEnd >= tvdbEpisode.EpisodeIndex)
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

        private async Task<List<Collection>> GetCollectionsAsync(CancellationToken cancellationToken)
        {
            await LogInformation("Asking Emby for all root folders");
            var rootItems = await _embyClient.GetMediaFoldersAsync(cancellationToken);

            return rootItems.Items.Select(x => new Collection
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.CollectionType.ToCollectionType(),
                PrimaryImage = x.ImageTags.ContainsKey(ImageType.Primary) ? x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value : default(string)
            }).ToList();
        }

        #endregion

        public void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}
