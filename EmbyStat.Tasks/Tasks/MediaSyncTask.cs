using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Api.Tvdb;
using EmbyStat.Api.Tvdb.Models;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Converters;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CollectionType = EmbyStat.Common.Models.CollectionType;

namespace EmbyStat.Tasks.Tasks
{
    public class MediaSyncTask : IScheduledTask
    {
        private readonly IEmbyClient _embyClient;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITvdbClient _tvdbClient;
        private Configuration _settings;
        private IProgressLogger _progressLogger;

        public MediaSyncTask(IApplicationBuilder app)
        {
            _embyClient = app.ApplicationServices.GetService<IEmbyClient>();
            _configurationRepository = app.ApplicationServices.GetService<IConfigurationRepository>();
            _movieRepository = app.ApplicationServices.GetService<IMovieRepository>();
            _genreRepository = app.ApplicationServices.GetService<IGenreRepository>();
            _personRepository = app.ApplicationServices.GetService<IPersonRepository>();
            _collectionRepository = app.ApplicationServices.GetService<ICollectionRepository>();
            _showRepository = app.ApplicationServices.GetService<IShowRepository>();
            _tvdbClient = app.ApplicationServices.GetService<ITvdbClient>();
        }
        
        public string Name => "TASKS.MEDIASYNCTITLE";
        public string Key => "MediaSync";
        public string Description => "TASKS.MEDIASYNCDESCRIPTION";
        public string Category => "Sync";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger progressLogger)
        {
            progress.Report(0);

            _progressLogger = progressLogger;
            _settings = _configurationRepository.GetConfiguration();
            if (!_settings.WizardFinished)
            {
                _progressLogger.LogWarning(Constants.LogPrefix.MediaSyncTask, "Media sync task not running because wizard is not finished yet!");
                return;
            }

            _embyClient.SetAddressAndUrl(_settings.EmbyServerAddress, _settings.AccessToken);

            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "First delete all existing media and root media collections from database so we have a clean start.");
            CleanUpDatabase();
            progress.Report(3);

            var rootItems = await GetRootItems(_settings.EmbyUserId, cancellationToken);
            _collectionRepository.AddOrUpdateRange(rootItems);
            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Found {rootItems.Count} root items, getting ready for processing");

            await ProcessMovies(rootItems, cancellationToken, progress);
            await ProcessShows(rootItems, cancellationToken, progress);
            await SyncMissingEpisodes(_settings.LastTvdbUpdate, _settings.TvdbApiKey, cancellationToken, progress);

            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Media syncronisation completed.");
            progress.Report(100);
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _showRepository.RemoveShows();
        }

        #region Movies
        private async Task ProcessMovies(IReadOnlyList<Collection> rootItems, CancellationToken cancellationToken, IProgress<double> progress)
        {
            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Lets start processing movies");
            progress.Report(5);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                var rootItem = rootItems[i];
                cancellationToken.ThrowIfCancellationRequested();
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Asking Emby all movies for library {rootItem.Name} ({rootItem.Type.ToString()})");
                var movies = (await GetMoviesFromEmby(rootItem.Id, cancellationToken)).ToList();
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask,
                    movies.Any()
                        ? $"We found {movies.Count} movies. Ready to add them to the database."
                        : "No movies found for this library. Moving on.");
                if (!movies.Any()) continue;

                movies.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));

                await ProcessGenresFromEmby(rootItem.Id, movies.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItem.Id, movies.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);

                var j = 0;
                foreach (var movie in movies.OrderBy(x => x.OriginalTitle))
                {
                    j++;
                    cancellationToken.ThrowIfCancellationRequested();
                    _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask,
                        $"Processing movie {movie.Name}");
                    _movieRepository.AddOrUpdate(movie);
                    progress.Report(
                        Math.Floor(5 + 25 / ((double) movies.Count / j * ((double) rootItems.Count / (i + 1)))));
                }
            }
        }

        private async Task<IEnumerable<Movie>> GetMoviesFromEmby(Guid parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = _settings.EmbyUserId,
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
            var movies = embyMovies.Items.Where(x => x.Type == Constants.Type.Movie).Select(MovieHelper.ConvertToMovie).ToList();

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
        private async Task ProcessShows(List<Collection> rootItems, CancellationToken cancellationToken, IProgress<double> progress)
        {
            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Lets start processing shows");
            progress.Report(33);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                var rootItem = rootItems[i];
                cancellationToken.ThrowIfCancellationRequested();
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Asking Emby all shows for parent {rootItem.Name}");
                var shows = await GetShowsFromEmby(rootItem.Id, cancellationToken);
                if(!shows.Any()) continue;

                shows.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));

                await ProcessGenresFromEmby(rootItem.Id, shows.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItem.Id, shows.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);
                _showRepository.AddRange(shows);

                var j = 0;
                foreach (var show in shows)
                {
                    j++;
                    progress.Report(Math.Floor(33 + 36 / ((double)shows.Count / j * (rootItems.Count / (double)(i + 1)))));
                    var rawSeasons = await GetSeasonsFromEmby(show.Id, cancellationToken);

                    var episodes = new List<Episode>();
                    var seasonLinks = new List<Tuple<Guid, Guid>>();
                    foreach (var season in rawSeasons)
                    {
                        var eps = await GetEpisodesFromEmby(season.Id, cancellationToken);
                        eps.ForEach(x => x.Collections.Add(new MediaCollection{CollectionId = rootItem.Id }));
                        episodes.AddRange(eps);

                        seasonLinks.AddRange(eps.Select(x => new Tuple<Guid, Guid>(season.Id, x.Id)));
                    }

                    _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Processing show {show.Name} with {rawSeasons.Count} seasons and {episodes.Count} episodes");

                    var groupedEpisodes = episodes.GroupBy(x => x.Id).Select(x => new { Episode = episodes.First(y => y.Id == x.Key) });

                    _showRepository.AddRange(groupedEpisodes.Select(x => x.Episode).ToList());

                    var seasons = rawSeasons.Select(x => ShowHelper.ConvertToSeason(x, seasonLinks.Where(y => y.Item1 == x.Id))).ToList();
                    seasons.ForEach(x => x.Collections.Add(new MediaCollection { CollectionId = rootItem.Id }));
                    _showRepository.AddRange(seasons);

                    cancellationToken.ThrowIfCancellationRequested();
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task SyncMissingEpisodes(DateTime? lastUpdateFromTvdb, string tvdbApiKey, CancellationToken cancellationToken, IProgress<double> progress)
        {
            await _tvdbClient.Login(tvdbApiKey, cancellationToken);

            var shows = _showRepository
                .GetAllShows(new Guid[]{})
                .Where(x => !string.IsNullOrWhiteSpace(x.TVDB))
                .ToList();

            var showsWithMissingEpisodes = shows.Where(x => !x.TvdbSynced).ToList();

            if (lastUpdateFromTvdb.HasValue)
            {
                var showsThatNeedAnUpdate = await _tvdbClient.GetShowsToUpdate(shows.Select(x => x.TVDB), lastUpdateFromTvdb.Value, cancellationToken);
                showsWithMissingEpisodes.AddRange(shows.Where(x => showsThatNeedAnUpdate.Any(y => y == x.TVDB)));
                showsWithMissingEpisodes = showsWithMissingEpisodes.DistinctBy(x => x.TVDB).ToList();
            }

            var now = DateTime.Now;
            await GetMissingEpisodesFromTvdb(showsWithMissingEpisodes, cancellationToken, progress);

            _settings.LastTvdbUpdate = now;
            _configurationRepository.Update(_settings);
        }

        private async Task GetMissingEpisodesFromTvdb(IEnumerable<Show> shows, CancellationToken cancellationToken, IProgress<double> progress)
        {
            double i = 0;
            var showCount = shows.Count();
            foreach (var show in shows)
            {
                i++;
                progress.Report(Math.Floor(66 + 33 / (showCount / i)));

                var neededEpisodeCount = 0;
                var seasons = _showRepository.GetAllSeasonsForShow(show.Id).ToList();
                var episodes = _showRepository.GetAllEpisodesForShow(show.Id, true).ToList();

                try
                {
                    var tvdbEpisodes = await _tvdbClient.GetEpisodes(show.TVDB, cancellationToken);

                    foreach (var episode in tvdbEpisodes)
                    {
                        var season = seasons.SingleOrDefault(x => x.IndexNumber == episode.SeasonIndex);
                        if (IsEpisodeMissing(episodes, season, episode))
                        {
                            neededEpisodeCount++;
                        }
                    }

                    _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Found {neededEpisodeCount} missing episodes for show {show.Name}");
                    show.TvdbSynced = true;
                    show.MissingEpisodesCount = neededEpisodeCount;
                    _showRepository.UpdateShow(show);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"{Constants.LogPrefix.MediaSyncTask}\tProcessing {show.Name} failed. Marking this show as failed");
                    show.TvdbFailed = true;
                    _showRepository.UpdateShow(show);
                }
            }
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

        private async Task<List<Show>> GetShowsFromEmby(Guid parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = _settings.EmbyUserId,
                IncludeItemTypes = new[] { "Series" },
                Fields = new[]
                {

                    ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People
                }
            };

            var embyShows = await _embyClient.GetItemsAsync(query, cancellationToken);

            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask,
                embyShows.TotalRecordCount == 0
                    ? "No TV shows found in this collection. Moving on to the next collection."
                    : $"Ready to add shows to database. We found {embyShows.TotalRecordCount} shows");
            return embyShows.Items.Select(ShowHelper.ConvertToShow).ToList();
        }

        private async Task<List<BaseItemDto>> GetSeasonsFromEmby(Guid parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = _settings.EmbyUserId,
                IncludeItemTypes = new[] { nameof(Season) },
                Fields = new[]
                {
                    ItemFields.DateCreated, ItemFields.Path, ItemFields.SortName, ItemFields.ParentId
                }
            };

            var embySeasons = await _embyClient.GetItemsAsync(query, cancellationToken);
            return embySeasons.Items.ToList();
        }

        private async Task<List<Episode>> GetEpisodesFromEmby(Guid parentId, CancellationToken cancellationToken)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "sortname" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                UserId = _settings.EmbyUserId,
                IncludeItemTypes = new[] { nameof(Episode) },
                Fields = new[]
                {
                    ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId
                }
            };

            var embyEpisodes = await _embyClient.GetItemsAsync(query, cancellationToken);
            return embyEpisodes.Items.Select(ShowHelper.ConvertToEpisode).ToList();
        }

        #endregion

        #region Helpers

        private async Task ProcessGenresFromEmby(Guid id, IEnumerable<Guid> genresNeeded, CancellationToken cancellationToken)
        {
            var query = new ItemsByNameQuery
            {
                ParentId = id,
                Recursive = true
            };

            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Asking Emby for all needed genres.");

            var embyGenres = await _embyClient.GetGenresAsync(query, cancellationToken);
            var existingGenres = _genreRepository.GetIds();

            var newGenres = embyGenres
                .Items
                .Where(x => genresNeeded.Any(y => y == x.Id))
                .Where(x => existingGenres.All(y => y != x.Id))
                .ToList();

            if (newGenres.Any())
            {
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Need to add {newGenres.Count} genres first ({string.Join(", ", newGenres.Select(x => x.Name))})");
                var genres = newGenres.DistinctBy(x => x.Id).Select(GenreHelper.ConvertToGenre);
                _genreRepository.AddRangeIfMissing(genres);
            }
            else
            {
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "No new genres to add");
            }
        }

        private async Task ProcessPeopleFromEmby(Guid id, IEnumerable<Guid> neededPeople, CancellationToken cancellationToken)
        {
            var query = new PersonsQuery
            {
                ParentId = id,
                Recursive = true
            };

            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Asking Emby for all needed people.");
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
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, $"Need to add {newPeople.Count} people first{extraLogText}");
                var people = newPeople.DistinctBy(x => x.Id).Select(PersonHelper.ConvertToSmallPerson);
                _personRepository.AddRangeIfMissing(people);
            }
            else
            {
                _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "No new people to add");
            }
        }

        private async Task<List<Collection>> GetRootItems(string id, CancellationToken cancellationToken)
        {
            _progressLogger.LogInformation(Constants.LogPrefix.MediaSyncTask, "Asking Emby for all root folders");
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

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ TaskKey = Key, TimeOfDayTicks = 0, Type = "DailyTrigger"}
            };
        }
    }
}
