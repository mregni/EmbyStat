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
            _progressLogger = progressLogger;
            _settings = _configurationRepository.GetConfiguration();
            if (!_settings.WizardFinished)
            {
                Log.Warning("Media sync task not running because wizard is not finished yet!");
                return;
            }

            _embyClient.SetAddressAndUrl(_settings.EmbyServerAddress, _settings.AccessToken);

            _progressLogger.LogInformation("First delete all existing media and root media collections from database so we have a clean start.");
            CleanUpDatabase();
            progress.Report(5);

            await ProcessMovies(cancellationToken, progress);
            await ProcessShows(cancellationToken, progress);
            await SyncMissingEpisodes(_settings.LastTvdbUpdate, _settings.TvdbApiKey, cancellationToken, progress);

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
            _progressLogger.LogInformation("Lets start processing movies");
            var rootItems = await GetRootItemsByType("movies", CollectionType.Movies, _settings.EmbyUserId, cancellationToken);
            _collectionRepository.AddCollectionRange(rootItems);
            _progressLogger.LogInformation($"Found {rootItems.Count} movie root items, getting ready for processing");
            progress.Report(12);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _progressLogger.LogInformation($"Asking Emby all movies for parent ({rootItems[i].Id}) {rootItems[i].Name}");
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
                    _progressLogger.LogInformation($"Processing movie ({movie.Id}) {movie.Name}");
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

            _progressLogger.LogInformation($"Ready to add movies to database. We found {embyMovies.TotalRecordCount} movies");
            return embyMovies.Items.Where(x => x.Type == Constants.Type.Movie).Select(MovieHelper.ConvertToMovie);
        }

        #endregion

        #region Shows
        private async Task ProcessShows(CancellationToken cancellationToken, IProgress<double> progress)
        {
            _progressLogger.LogInformation("Lets start processing shows");
            var rootItems = await GetRootItemsByType("tvshows", CollectionType.TvShow, _settings.EmbyUserId, cancellationToken);
            _collectionRepository.AddCollectionRange(rootItems);
            _progressLogger.LogInformation($"Found {rootItems.Count} show root items, getting ready for processing");
            progress.Report(55);

            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < rootItems.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _progressLogger.LogInformation($"Asking Emby all shows for parent ({rootItems[i].Id}) {rootItems[i].Name}");
                var shows = await GetShowsFromEmby(rootItems[i].Id, cancellationToken);
                shows.ForEach(x => x.CollectionId = rootItems[i].Id);

                await ProcessGenresFromEmby(rootItems[i].Id, shows.SelectMany(x => x.MediaGenres, (movie, genre) => genre.GenreId), cancellationToken);
                await ProcessPeopleFromEmby(rootItems[i].Id, shows.SelectMany(x => x.ExtraPersons, (movie, person) => person.PersonId), cancellationToken);
                _showRepository.AddRange(shows);

                var j = 0;
                foreach (var show in shows)
                {
                    j++;
                    progress.Report(Math.Floor(55 + (double)45 / shows.Count * j / rootItems.Count * i));
                    var rawSeasons = await GetSeasonsFromEmby(show.Id, cancellationToken);

                    var episodes = new List<Episode>();
                    var seasonLinks = new List<Tuple<string, string>>();
                    foreach (var season in rawSeasons)
                    {
                        var eps = await GetEpisodesFromEmby(season.Id, cancellationToken);
                        eps.ForEach(x => x.CollectionId = rootItems[i].Id);
                        episodes.AddRange(eps);

                        seasonLinks.AddRange(eps.Select(x => new Tuple<string, string>(season.Id, x.Id)));
                    }

                    _progressLogger.LogInformation($"Processing show ({show.Id}) {show.Name} with {rawSeasons.Count} seasons and {episodes.Count} episodes");

                    var groupedEpisodes = episodes.GroupBy(x => x.Id).Select(x => new { Episode = episodes.First(y => y.Id == x.Key) });

                    _showRepository.AddRange(groupedEpisodes.Select(x => x.Episode).ToList());

                    var seasons = rawSeasons.Select(x => ShowHelper.ConvertToSeason(x, seasonLinks.Where(y => y.Item1 == x.Id))).ToList();
                    seasons.ForEach(x => x.CollectionId = rootItems[i].Id);
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
                .GetAllShows(new string[]{})
                .Where(x => !string.IsNullOrWhiteSpace(x.TVDB))
                .ToList();

            var showsWithMissingEpisodes = shows.Where(x => !x.TvdbSynced).ToList();

            if (lastUpdateFromTvdb.HasValue)
            {
                var showsThatNeedAnUpdate = await _tvdbClient.GetShowsToUpdate(shows.Select(x => x.TVDB), lastUpdateFromTvdb.Value, cancellationToken);
                showsWithMissingEpisodes.AddRange(shows.Where(x => showsThatNeedAnUpdate.Any(y => y == x.TVDB)));
                showsWithMissingEpisodes = showsWithMissingEpisodes.DistinctBy(x => x.TVDB).ToList();
            }

            await GetMissingEpisodesFromTvdb(showsWithMissingEpisodes, cancellationToken);
        }

        private async Task GetMissingEpisodesFromTvdb(IEnumerable<Show> shows, CancellationToken cancellationToken)
        {
            foreach (var show in shows)
            {
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

                    _progressLogger.LogInformation($"Found {neededEpisodeCount} missing episodes for show {show.Name}");
                    show.TvdbSynced = true;
                    show.MissingEpisodesCount = neededEpisodeCount;
                    _showRepository.UpdateShow(show);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Processing {show.Name} failed. Marking this show as failed");
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

            _progressLogger.LogInformation($"Ready to add shows to database. We found {embyShows.TotalRecordCount} shows");
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
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId
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
                _progressLogger.LogInformation($"Need to add {newGenres.Count} genres first ({string.Join(", ", newGenres.Select(x => x.Name))})");
                var genres = newGenres.DistinctBy(x => x.Id).Select(GenreHelper.ConvertToGenre);
                _genreRepository.AddRangeIfMissing(genres);
            }
            else
            {
                _progressLogger.LogInformation("No new genres to add");
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
                _progressLogger.LogInformation($"Need to add {newPeople.Count} people first");
                var people = newPeople.DistinctBy(x => x.Id).Select(PersonHelper.ConvertToSmallPerson);
                _personRepository.AddRangeIfMissing(people);
            }
            else
            {
                _progressLogger.LogInformation("No new people to add");
            }
        }

        private async Task<List<Collection>> GetRootItemsByType(string type, CollectionType collectionType, string id, CancellationToken cancellationToken)
        {
            _progressLogger.LogInformation($"Asking for all root views for admin user with id {id}");
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
