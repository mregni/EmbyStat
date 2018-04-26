using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Converters;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CollectionType = EmbyStat.Common.Models.CollectionType;

namespace EmbyStat.Tasks.Tasks
{
    public class MovieSyncTask : IScheduledTask
    {
        private readonly IEmbyClient _embyClient;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ICollectionRepository _collectionRepository;
        private Configuration _settings;

        public MovieSyncTask(IApplicationBuilder app)
        {
            _embyClient = app.ApplicationServices.GetService<IEmbyClient>();
            _configurationRepository = app.ApplicationServices.GetService<IConfigurationRepository>();
            _movieRepository = app.ApplicationServices.GetService<IMovieRepository>();
            _genreRepository = app.ApplicationServices.GetService<IGenreRepository>();
            _personRepository = app.ApplicationServices.GetService<IPersonRepository>();
            _collectionRepository = app.ApplicationServices.GetService<ICollectionRepository>();
        }

        public string Name => "Movie sync";
        public string Key => "MovieSync";
        public string Description => "TASKS.MOVIESYNCDESCRIPTION";
        public string Category => "Sync";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            _settings = _configurationRepository.GetSingle();
            if (!_settings.WizardFinished)
            {
                Log.Warning("Movie sync task not running because wizard is not finished yet!");
                return;
            }

            _embyClient.SetAddressAndUrl(_settings.EmbyServerAddress, _settings.AccessToken);

            Log.Information("First delete all existing movies and root movie collections from database so we have a clean start.");
            CleanUpDatabase();
            progress.Report(10);

            var rootItems = await GetMovieRootItems(_settings.EmbyUserId, cancellationToken);
            AddRootItemsToDb(rootItems);
            Log.Information($"Found {rootItems.Count} root items, getting ready for processing");
            progress.Report(20);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information($"Asking Emby all movies for parent ({rootItems[i].Id}) {rootItems[i].Name}");
                var movies = (await GetMoviesFromEmby(rootItems[i].Id, cancellationToken)).ToList();
                movies.ForEach(x => x.CollectionId = rootItems[i].Id);

                await ProcessGenresFromEmby(rootItems[i].Id, movies.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                progress.Report(Math.Round(20 + (80 / (double)rootItems.Count * i) + (80 / (double)rootItems.Count * i) / 4));
                await ProcessPeopleFromEmby(rootItems[i].Id, movies.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);
                progress.Report(Math.Round(20 + (80 / (double)rootItems.Count * i) + (80 / (double)rootItems.Count * i) / 2));

                var j = 0;
                foreach (var movie in movies)
                {
                    j++;
                    cancellationToken.ThrowIfCancellationRequested();
                    Log.Information($"Processing movie ({movie.Id}) {movie.Name}");
                    AddMoviesToDb(movie);
                    progress.Report(Math.Round(20 + (80 / (double)rootItems.Count * i) + ((80 / (double)rootItems.Count * i) / 2) / movies.Count * j));
                }
            }
        }

        private void CleanUpDatabase()
        {
            _movieRepository.RemoveMovies();
            _collectionRepository.RemoveCollectionByType(CollectionType.Movies);
        }

        private async Task<List<Collection>> GetMovieRootItems(string id, CancellationToken cancellationToken)
        {
            Log.Information($"Asking for all root views for admin user with id {id}");
            var rootItem = await _embyClient.GetRootFolderAsync(id, cancellationToken);

            var items = await _embyClient.GetItemsAsync(new ItemQuery
            {
                ParentId = rootItem.Id,
                UserId = id
            }, cancellationToken);

            return items.Items.Where(x => x.CollectionType == "movies").Select(x => new Collection
            {
                Id = x.Id,
                Name = x.Name,
                Type = CollectionType.Movies,
                PrimaryImage = x.ImageTags.ContainsKey(ImageType.Primary) ? x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value : default(string)
            }).ToList();
        }

        private void AddMoviesToDb(Movie movie)
        {
            _movieRepository.Add(movie);
        }

        private void AddRootItemsToDb(IEnumerable<Collection> collections)
        {
            _collectionRepository.AddCollectionRange(collections);
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
                var genres = newGenres.Select(GenreHelper.ConvertToGenre);
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
                var people = newPeople.Select(PersonHelper.ConvertToPerson);
                _personRepository.AddRangeIfMissing(people);
            }
            else
            {
                Log.Information("No new people to add");
            }
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ TaskKey = Key, TimeOfDayTicks = 0, Type = "DailyTrigger"}
            };
        }
    }
}
