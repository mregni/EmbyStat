using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Api.Tvdb;
using EmbyStat.Common.Models;
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

        public string Name => "Media sync";
        public string Key => "MediaSync";
        public string Description => "TASKS.MEDIASYNCDESCRIPTION";
        public string Category => "Sync";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            _settings = _configurationRepository.GetSingle();
            if (!_settings.WizardFinished)
            {
                Log.Warning("Media sync task not running because wizard is not finished yet!");
                return;
            }

            _embyClient.SetAddressAndUrl(_settings.EmbyServerAddress, _settings.AccessToken);

            Log.Information("First delete all existing media and root media collections from database so we have a clean start.");
            CleanUpDatabase();
            progress.Report(5);

            await ProcessMovies(cancellationToken, progress);
            await ProcessShows(cancellationToken, progress);
            await SyncMissingEpisodes(cancellationToken, progress, _settings.LastTvdbUpdate);

            progress.Report(100);
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _showRepository.RemoveShows();
            _collectionRepository.RemoveCollectionByType(CollectionType.Movies);
            _collectionRepository.RemoveCollectionByType(CollectionType.TvShow);
        }

        #region Movies
        private async Task ProcessMovies(CancellationToken cancellationToken, IProgress<double> progress)
        {
            Log.Information("Lets start processing movies");
            var rootItems = await GetRootItemsByType("movies", CollectionType.Movies, _settings.EmbyUserId, cancellationToken);
            _collectionRepository.AddCollectionRange(rootItems);
            Log.Information($"Found {rootItems.Count} movie root items, getting ready for processing");
            progress.Report(12);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information($"Asking Emby all movies for parent ({rootItems[i].Id}) {rootItems[i].Name}");
                var movies = (await GetMoviesFromEmby(rootItems[i].Id, cancellationToken)).ToList();
                movies.ForEach(x => x.CollectionId = rootItems[i].Id);

                await ProcessGenresFromEmby(rootItems[i].Id, movies.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItems[i].Id, movies.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);

                progress.Report(15);
                var j = 0;
                foreach (var movie in movies)
                {
                    j++;
                    cancellationToken.ThrowIfCancellationRequested();
                    Log.Information($"Processing movie ({movie.Id}) {movie.Name}");
                    _movieRepository.Add(movie);
                    progress.Report(Math.Floor(15 + (double)30 / movies.Count * j / rootItems.Count * i));
                }
            }
        }

        private async Task<IEnumerable<Movie>> GetMoviesFromEmby(string parentId, CancellationToken cancellationToken)
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
                    ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.HomePageUrl,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                    ItemFields.People
                }
            };

            var embyMovies = await _embyClient.GetItemsAsync(query, cancellationToken);

            Log.Information($"Ready to add movies to database. We found {embyMovies.TotalRecordCount} movies");
            return embyMovies.Items.Select(MovieHelper.ConvertToMovie);
        }

        #endregion

        #region Shows
        private async Task ProcessShows(CancellationToken cancellationToken, IProgress<double> progress)
        {
            Log.Information("Lets start processing shows");
            var rootItems = await GetRootItemsByType("tvshows", CollectionType.TvShow, _settings.EmbyUserId, cancellationToken);
            _collectionRepository.AddCollectionRange(rootItems);
            Log.Information($"Found {rootItems.Count} show root items, getting ready for processing");
            progress.Report(55);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information($"Asking Emby all shows for parent ({rootItems[i].Id}) {rootItems[i].Name}");
                var shows = await GetShowsFromEmby(rootItems[i].Id, cancellationToken);
                shows.ForEach(x => x.CollectionId = rootItems[i].Id);

                await ProcessGenresFromEmby(rootItems[i].Id, shows.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItems[i].Id, shows.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);
                _showRepository.AddRange(shows);

                var j = 0;
                foreach (var show in shows)
                {
                    j++;
                    progress.Report(Math.Floor(55 + (double)45 / shows.Count * j / rootItems.Count * i ));
                    var rawSeasons = await GetSeasonsFromEmby(show.Id, cancellationToken);

                    var episodes = new List<Episode>();
                    foreach (var season in rawSeasons)
                    {
                        var eps = await GetEpisodesFromEmby(season.Id, cancellationToken);
                        episodes.AddRange(eps);
                    }

                    Log.Information($"Processing show ({show.Id}) {show.Name} with {rawSeasons.Count} seasons and {episodes.Count} episodes");
                    episodes.ForEach(x => x.CollectionId = rootItems[i].Id);
                    var groupedEpisodes = episodes.GroupBy(x => x.Id).Select(x => new { Episode = episodes.First(y => y.Id == x.Key) });
                    _showRepository.AddRange(groupedEpisodes.Select(x => x.Episode).ToList());

                    var seasons = rawSeasons.Select(x => ShowHelper.ConvertToSeason(x, episodes.Where(y => y.ParentId == x.Id))).ToList();
                    seasons.ForEach(x => x.CollectionId = rootItems[i].Id);
                    _showRepository.AddRange(seasons);

                    cancellationToken.ThrowIfCancellationRequested();
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task SyncMissingEpisodes(CancellationToken cancellationToken, IProgress<double> progress, DateTime? lastUpdateFromTvdb)
        {
            var shows = _showRepository.GetAllShows();

            var tvdbIds = shows.Where(x => !string.IsNullOrWhiteSpace(x.TVDB)).Select(x => x.TVDB);


        }

        private async Task<List<Show>> GetShowsFromEmby(string parentId, CancellationToken cancellationToken)
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

                    ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.HomePageUrl,
                    ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People
                }
            };

            var embyShows = await _embyClient.GetItemsAsync(query, cancellationToken);

            Log.Information($"Ready to add shows to database. We found {embyShows.TotalRecordCount} shows");
            return embyShows.Items.Select(ShowHelper.ConvertToShow).ToList();
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

        private async Task<List<Episode>> GetEpisodesFromEmby(string parentId, CancellationToken cancellationToken)
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
                    ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.HomePageUrl,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId, ItemFields.People
                }
            };

            var embyEpisodes = await _embyClient.GetItemsAsync(query, cancellationToken);
            return embyEpisodes.Items.Select(ShowHelper.ConvertToEpisode).ToList();
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

            var embyGenres = await _embyClient.GetGenresAsync(query, cancellationToken);
            var existingGenres = _genreRepository.GetIds();

            var newGenres = embyGenres
                .Items
                .Where(x => genresNeeded.Any(y => y == x.Id))
                .Where(x => existingGenres.All(y => y != x.Id))
                .ToList();

            if (newGenres.Any())
            {
                Log.Information($"Need to add {newGenres.Count} genres first ({string.Join(", ", newGenres.Select(x => x.Name))})");
                var genres = newGenres.DistinctBy(x => x.Id).Select(GenreHelper.ConvertToGenre);
                _genreRepository.AddRangeIfMissing(genres);
            }
            else
            {
                Log.Information("No new genres to add");
            }
        }

        private async Task ProcessPeopleFromEmby(string id, IEnumerable<string> neededPeople, CancellationToken cancellationToken)
        {
            var query = new PersonsQuery
            {
                ParentId = id,
                Recursive = true
            };

            var embyPeople = await _embyClient.GetPeopleAsync(query, cancellationToken);

            var existingPeople = _personRepository.GetIds();
            var newPeople = embyPeople
                .Items
                .Where(x => neededPeople.Any(y => y == x.Id))
                .Where(x => existingPeople.All(y => y != x.Id))
                .ToList();

            if (newPeople.Any())
            {
                Log.Information($"Need to add {newPeople.Count} people first");
                var people = newPeople.DistinctBy(x => x.Id).Select(PersonHelper.ConvertToSmallPerson);
                _personRepository.AddRangeIfMissing(people);
            }
            else
            {
                Log.Information("No new people to add");
            }
        }

        private async Task<List<Collection>> GetRootItemsByType(string type, CollectionType collectionType, string id, CancellationToken cancellationToken)
        {
            Log.Information($"Asking for all root views for admin user with id {id}");
            var rootItem = await _embyClient.GetRootFolderAsync(id, cancellationToken);

            var items = await _embyClient.GetItemsAsync(new ItemQuery
            {
                ParentId = rootItem.Id,
                UserId = id
            }, cancellationToken);

            return items.Items.Where(x => x.CollectionType == type).Select(x => new Collection
            {
                Id = x.Id,
                Name = x.Name,
                Type = collectionType,
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
