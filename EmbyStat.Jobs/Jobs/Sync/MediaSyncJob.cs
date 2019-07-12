using System;
using System.Collections;
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
        private readonly Logger _logger;

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
            _logger = LogManager.GetCurrentClassLogger();
        }

        public sealed override Guid Id => Constants.JobIds.MediaSyncId;
        public override string JobPrefix => Constants.LogPrefix.MediaSyncJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            var cancellationToken = new CancellationToken(false);

            if (!Settings.WizardFinished)
            {
                await LogWarning("Media sync task not running because wizard is not yet finished!");
                return;
            }

            if (!await IsEmbyAlive(cancellationToken))
            {
                await LogWarning($"Halting task because we can't contact the Emby server on {Settings.FullEmbyServerAddress}, please check the connection and try again.");
                return;
            }

            await LogInformation("First delete all existing media and root media collections from database so we have a clean start.");
            var oldShows = _showRepository.GetAllShows(new string[] { }, false, true).ToList();
            CleanUpDatabase();
            await LogProgress(3);

            var collections = await GetCollections(cancellationToken);
            _collectionRepository.AddOrUpdateRange(collections);
            await LogInformation($"Found {collections.Count} root items, getting ready for processing");

            await ProcessPeople(cancellationToken);

            await ProcessMovies(collections, cancellationToken);

            await ProcessShows(collections, oldShows, cancellationToken);
            await SyncMissingEpisodes(oldShows, cancellationToken);

            _statisticsRepository.MarkShowTypesAsInvalid();
            _statisticsRepository.MarkMovieTypesAsInvalid();
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _showRepository.RemoveShows();
        }

        private async Task<bool> IsEmbyAlive(CancellationToken cancellationToken)
        {
            var result = await _embyClient.PingEmbyAsync(Settings.FullEmbyServerAddress, cancellationToken);
            return result == "Emby Server";
        }

        private async Task ProcessPeople(CancellationToken cancellationToken)
        {
            var embyPeople = await _embyClient.GetPeopleAsync(new PersonsQuery(), cancellationToken);
            await LogInformation($"Need to add/update {embyPeople.TotalRecordCount} people first.");

            var people = embyPeople.Items.DistinctBy(x => x.Id).Select(PersonConverter.ConvertToSmallPerson);
            _personRepository.UpserRange(people);
        }

        #region Movies
        private async Task ProcessMovies(IReadOnlyList<Collection> collections, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            await LogProgress(5);

            var neededCollections = collections.Where(x => Settings.MovieCollectionTypes.Any(y => y == x.Type));

            foreach (var collection in neededCollections)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var movies = await PerformMovieSync(collection.Id);
                _movieRepository.UpsertRange(movies);
            }
        }

        private async Task<IEnumerable<Movie>> PerformMovieSync(string collectionId)
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
                        recursiveMovies.AddRange(await PerformMovieSync(parent.Id));
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
        private async Task ProcessShows(List<Collection> collections, IReadOnlyList<Show> oldSHows, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing shows");
            await LogProgress(33);

            var neededCollections = collections.Where(x => Settings.ShowCollectionTypes.Any(y => y == x.Type));

            foreach (var collection in neededCollections)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await PerformShowSync(oldSHows, collection.Id);
            }
        }

        private async Task PerformShowSync(IReadOnlyList<Show> oldShows, string collectionId)
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
            shows = null;
        }

        private async Task SyncMissingEpisodes(IReadOnlyList<Show> oldShows, CancellationToken cancellationToken)
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
            await GetMissingEpisodesFromTvdb(showsWithMissingEpisodes, cancellationToken);

            Settings.Tvdb.LastUpdate = now;
            await SettingsService.SaveUserSettings(Settings);
        }

        private async Task GetMissingEpisodesFromTvdb(IEnumerable<Show> shows, CancellationToken cancellationToken)
        {
            foreach (var show in shows)
            {
                try
                {
                    await ProgressMissingEpisodes(show, cancellationToken);
                }
                catch (Exception e)
                {
                    show.TvdbFailed = true;
                    _showRepository.UpdateShow(show);
                }
            }
        }

        private async Task ProgressMissingEpisodes(Show show, CancellationToken cancellationToken)
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

        private async Task<List<Collection>> GetCollections(CancellationToken cancellationToken)
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
