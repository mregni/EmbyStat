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
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Joins;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MediaBrowser.Model.Dto;
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
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITvdbClient _tvdbClient;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly Logger _logger;

        public MediaSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
            IEmbyClient embyClient, IMovieRepository movieRepository, IShowRepository showRepository, IGenreRepository genreRepository, 
            IPersonRepository personRepository, ICollectionRepository collectionRepository, ITvdbClient tvdbClient, 
            IStatisticsRepository statisticsRepository): base(hubHelper, jobRepository, settingsService)
        {
            _embyClient = embyClient;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _genreRepository = genreRepository;
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
            CleanUpDatabase();
            await LogProgress(3);

            var rootItems = await GetRootItems(cancellationToken);
            _collectionRepository.AddOrUpdateRange(rootItems);
            await LogInformation($"Found {rootItems.Count} root items, getting ready for processing");

            //await ProcessMovies(rootItems, cancellationToken);
            await ProcessShows(rootItems, cancellationToken);
            await SyncMissingEpisodes(Settings.Tvdb.LastUpdate, Settings.Tvdb.ApiKey, cancellationToken);

            await _statisticsRepository.MarkShowTypesAsInvalid();
            await _statisticsRepository.MarkMovieTypesAsInvalid();
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _showRepository.RemoveShows();
        }

        private async Task<bool> IsEmbyAlive(CancellationToken cancellationToken)
        {
            var result = await _embyClient.PingEmbyAsync(cancellationToken);
            return result == "Emby Server";
        }

        #region Movies
        private async Task ProcessMovies(IReadOnlyList<Collection> rootItems, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            await LogProgress(5);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                var rootItem = rootItems[i];

                if (!Settings.MovieCollectionTypes.Contains(rootItem.Type))
                {
                    await LogInformation($"Skipping collection {rootItem.Name} ({rootItem.Type.ToString()}) because it's not a wanted movie collection type.");
                    continue;;
                }

                cancellationToken.ThrowIfCancellationRequested();
                await LogInformation($"Asking Emby all movies for library {rootItem.Name} ({rootItem.Type.ToString()})");
                var movies = (await GetMoviesFromEmby(rootItem.Id, cancellationToken)).ToList();

                if (!movies.Any())
                {
                    await LogInformation("No movies found for this library. Moving on.");
                    continue;
                }

                await LogInformation($"We found {movies.Count} movies. Ready to add them to the database.");
                movies.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));

                await ProcessGenresFromEmby(rootItem.Id, movies.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItem.Id, movies.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);

                var j = 0;
                foreach (var movie in movies.OrderBy(x => x.OriginalTitle))
                {
                    j++;
                    cancellationToken.ThrowIfCancellationRequested();
                    await LogInformation($"Processing movie ({j}/{movies.Count}) {movie.Name}");
                    _movieRepository.AddOrUpdate(movie);
                    await LogProgress(Math.Floor(5 + 25 / ((double)movies.Count / j * ((double)rootItems.Count / (i + 1)))));
                }
            }
        }

        private async Task<IEnumerable<Movie>> GetMoviesFromEmby(string parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
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

            var embyMovies = await _embyClient.GetItemsAsync(query, cancellationToken);
            var movies = embyMovies.Items.Where(x => x.Type == Constants.Type.Movie).Select(MovieConverter.ConvertToMovie).ToList();

            var recursiveMovies = new List<Movie>();
            if (embyMovies.Items.Any(x => x.Type == Constants.Type.Boxset))
            {

                foreach (var parent in embyMovies.Items.Where(x => x.Type == Constants.Type.Boxset))
                {
                    recursiveMovies.AddRange(await GetMoviesFromEmby(parent.Id, cancellationToken));
                }
            }

            movies.AddRange(recursiveMovies);

            return movies.DistinctBy(x => x.Id);
        }

        #endregion

        #region Shows
        private async Task ProcessShows(List<Collection> rootItems, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing shows");
            await LogProgress(33);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                var rootItem = rootItems[i];

                if (!Settings.ShowCollectionTypes.Contains(rootItem.Type))
                {
                    await LogInformation($"Skipping collection {rootItem.Name} ({rootItem.Type.ToString()}) because it's not a wanted show collection type.");
                    continue; ;
                }

                cancellationToken.ThrowIfCancellationRequested();
                await LogInformation($"Asking Emby all shows for parent {rootItem.Name}");
                var shows = await GetShowsFromEmby(rootItem.Id, cancellationToken);

                if (!shows.Any())
                {
                    await LogInformation($"No shows found for library {rootItem.Name}.");
                    continue;
                }

                await LogInformation($"We found {shows.Count} shows. Ready to add them to the database.");
                shows.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));

                await ProcessGenresFromEmby(rootItem.Id, shows.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItem.Id, shows.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);
                _showRepository.AddRange(shows);

                var j = 0;
                foreach (var show in shows)
                {
                    j++;
                    await LogProgress(Math.Floor(33 + 36 / ((double)shows.Count / j * (rootItems.Count / (double)(i + 1)))));
                    await ProcessShow(show, rootItem, j, shows.Count, cancellationToken);
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task ProcessShow(Show show, Collection rootItem, int index, int showCount, CancellationToken cancellationToken)
        {
            var rawSeasons = await GetSeasonsFromEmby(show.Id, cancellationToken);

            var episodes = new List<Episode>();
            var seasonLinks = new List<Tuple<string, string>>();
            foreach (var season in rawSeasons)
            {
                var eps = await GetEpisodesFromEmby(season.Id, cancellationToken);
                eps.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));
                episodes.AddRange(eps);

                seasonLinks.AddRange(eps.Select(x => new Tuple<string, string>(season.Id, x.Id)));
            }

            await LogInformation($"Processing show ({index}/{showCount})  {show.Name} with {rawSeasons.Count} seasons and {episodes.Count} episodes");

            var groupedEpisodes = episodes.GroupBy(x => x.Id).Select(x => new { Episode = episodes.First(y => y.Id == x.Key) });

            _showRepository.AddRange(groupedEpisodes.Select(x => x.Episode).ToList());

            var seasons = rawSeasons.Select(x => ShowConverter.ConvertToSeason(x, seasonLinks.Where(y => y.Item1 == x.Id))).ToList();
            seasons.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));
            _showRepository.AddRange(seasons);

            cancellationToken.ThrowIfCancellationRequested();
        }

        private async Task SyncMissingEpisodes(DateTime? lastUpdateFromTvdb, string tvdbApiKey, CancellationToken cancellationToken)
        {
            await LogInformation("Started checking missing episodes");
            await _tvdbClient.Login(tvdbApiKey, cancellationToken);

            var shows = _showRepository
                .GetAllShows(new string[] { })
                .Where(x => !string.IsNullOrWhiteSpace(x.TVDB))
                .ToList();

            var showsWithMissingEpisodes = shows.Where(x => !x.TvdbSynced).ToList();

            if (lastUpdateFromTvdb.HasValue)
            {
                var showsThatNeedAnUpdate = await _tvdbClient.GetShowsToUpdate(shows.Select(x => x.TVDB), lastUpdateFromTvdb.Value, cancellationToken);
                showsWithMissingEpisodes.AddRange(shows.Where(x => showsThatNeedAnUpdate.Any(y => y == x.TVDB)));
            }

            var now = DateTime.Now;
            await GetMissingEpisodesFromTvdb(showsWithMissingEpisodes.DistinctBy(x => x.TVDB).ToList(), cancellationToken);

            Settings.Tvdb.LastUpdate = now;
            await SettingsService.SaveUserSettings(Settings);
        }

        private async Task GetMissingEpisodesFromTvdb(List<Show> shows, CancellationToken cancellationToken)
        {
            double i = 0;

            var showCount = shows.Count;
            foreach (var show in shows)
            {
                i++;
                await LogProgress(Math.Floor(66 + 33 / (showCount / i)));

                var seasons = _showRepository.GetAllSeasonsForShow(show.Id).ToList();
                var episodes = _showRepository.GetAllEpisodesForShow(show.Id, true).ToList();

                try
                {
                    await ProgressMissingEpisodes(show, seasons, episodes, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"{Constants.LogPrefix.MediaSyncJob}\tProcessing {show.Name} failed. Marking this show as failed");
                    show.TvdbFailed = true;
                    _showRepository.UpdateShow(show);
                }
            }
        }

        private async Task ProgressMissingEpisodes(Show show, List<Season> seasons, List<Episode> episodes, CancellationToken cancellationToken)
        {
            var neededEpisodeCount = 0;
            var tvdbEpisodes = await _tvdbClient.GetEpisodes(show.TVDB, cancellationToken);

            foreach (var episode in tvdbEpisodes)
            {
                var season = seasons.SingleOrDefault(x => x.IndexNumber == episode.SeasonIndex);
                if (IsEpisodeMissing(episodes, season, episode))
                {
                    neededEpisodeCount++;
                }
            }

            await LogInformation($"Found {neededEpisodeCount} missing episodes for show {show.Name}");
            show.TvdbSynced = true;
            show.MissingEpisodesCount = neededEpisodeCount;
            _showRepository.UpdateShow(show);
        }

        private bool IsEpisodeMissing(List<Episode> localEpisodes, Season season, VirtualEpisode tvdbEpisode)
        {
            if (season == null)
            {
                return true;
            }

            foreach (var localEpisode in localEpisodes)
            {
                if (localEpisode.SeasonEpisodes.Any(y => y.SeasonId == season.Id))
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

        private async Task<List<Show>> GetShowsFromEmby(string parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { "Series" },
                Fields = new[]
                {

                    ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People
                }
            };

            var embyShows = await _embyClient.GetItemsAsync(query, cancellationToken);
            embyShows.Items = embyShows.Items.Where(x => x.Type == "Series").ToArray();

            await LogInformation(embyShows.TotalRecordCount == 0
                ? "No TV shows found in this collection. Moving on to the next collection."
                : $"Ready to add shows to database. We found {embyShows.TotalRecordCount} shows");

            return embyShows.Items.Select(ShowConverter.ConvertToShow).ToList();
        }

        private async Task<List<BaseItemDto>> GetSeasonsFromEmby(string parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { nameof(Season) },
                Fields = new[]
                {
                    ItemFields.DateCreated, ItemFields.Path, ItemFields.SortName, ItemFields.ParentId
                }
            };

            var embySeasons = await _embyClient.GetItemsAsync(query, cancellationToken);
            return embySeasons.Items.ToList();
        }

        private async Task<List<Episode>> GetEpisodesFromEmby(string parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = Settings.Emby.UserId,
                IncludeItemTypes = new[] { nameof(Episode) },
                Fields = new[]
                {
                    ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId
                }
            };

            var embyEpisodes = await _embyClient.GetItemsAsync(query, cancellationToken);
            return embyEpisodes.Items.Select(ShowConverter.ConvertToEpisode).ToList();
        }

        #endregion

        #region Helpers

        private async Task ProcessGenresFromEmby(string id, IEnumerable<string> genresNeeded, CancellationToken cancellationToken)
        {
            var query = new ItemsByNameQuery
            {
                ParentId = id,
                Recursive = true
            };

            await LogInformation("Asking Emby for all needed genres.");

            var embyGenres = await _embyClient.GetGenresAsync(query, cancellationToken);
            var existingGenres = _genreRepository.GetIds();

            var newGenres = embyGenres
                .Items
                .Where(x => genresNeeded.Any(y => y == x.Id))
                .Where(x => existingGenres.All(y => y != x.Id))
                .ToList();

            if (newGenres.Any())
            {
                await LogInformation($"Need to add {newGenres.Count} genres first ({string.Join(", ", newGenres.Select(x => x.Name))})");
                var genres = newGenres.DistinctBy(x => x.Id).Select(GenreConverter.ConvertToGenre);
                _genreRepository.AddRangeIfMissing(genres);
            }
            else
            {
                await LogInformation("No new genres to add");
            }
        }

        private async Task ProcessPeopleFromEmby(string id, IEnumerable<string> neededPeople, CancellationToken cancellationToken)
        {
            var query = new PersonsQuery
            {
                ParentId = id,
                Recursive = true
            };

            await LogInformation("Asking Emby for all needed people.");
            var embyPeople = await _embyClient.GetPeopleAsync(query, cancellationToken);

            var existingPeople = _personRepository.GetIds();
            var newPeople = embyPeople
                .Items
                .Where(x => neededPeople.Any(y => y == x.Id))
                .Where(x => existingPeople.All(y => y != x.Id))
                .ToList();

            if (newPeople.Any())
            {
                var extraLogText = newPeople.Count > 100 ? ", this can take some time." : "";
                await LogInformation($"Need to add {newPeople.Count} people first{extraLogText}");
                var people = newPeople.DistinctBy(x => x.Id).Select(PersonConverter.ConvertToSmallPerson);
                _personRepository.AddRangeIfMissing(people);
            }
            else
            {
                await LogInformation("No new people to add");
            }
        }

        private async Task<List<Collection>> GetRootItems(CancellationToken cancellationToken)
        {
            await LogInformation("Asking Emby for all root folders");
            var rootItems = await _embyClient.GetMediaFolders(cancellationToken);

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
