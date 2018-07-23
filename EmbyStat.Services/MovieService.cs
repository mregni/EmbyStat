using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using CollectionType = EmbyStat.Common.Models.CollectionType;
using Constants = EmbyStat.Common.Constants;

namespace EmbyStat.Services
{
    public class MovieService : MediaService, IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonService _personService;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, IGenreRepository genreRepository, IPersonService personService, IConfigurationRepository configurationRepository, IStatisticsRepository statisticsRepository, ITaskRepository taskRepository)
        : base(taskRepository)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
            _configurationRepository = configurationRepository;
            _statisticsRepository = statisticsRepository;
        }

        public IEnumerable<Collection> GetMovieCollections()
        {
            var config = _configurationRepository.GetConfiguration();
            return _collectionRepository.GetCollectionByTypes(config.MovieCollectionTypes);
        }

        public MovieStats GetGeneralStatsForCollections(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.MovieGeneral);

            MovieStats stats;
            if (NewStatisticsNeeded(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<MovieStats>(statistic.JsonResult);
            }
            else
            {
                var movies = _movieRepository.GetAll(collectionIds, true);

                stats = new MovieStats
                {
                    MovieCount = TotalMovieCount(movies),
                    GenreCount = TotalMovieGenres(movies),
                    TotalPlayableTime = TotalPlayLength(movies),
                    HighestRatedMovie = HighestRatedMovie(movies),
                    LowestRatedMovie = LowestRatedMovie(movies),
                    OldestPremieredMovie = OldestPremieredMovie(movies),
                    YoungestPremieredMovie = YoungestPremieredMovie(movies),
                    ShortestMovie = ShortestMovie(movies),
                    LongestMovie = LongestMovie(movies),
                    YoungestAddedMovie = YoungestAddedMovie(movies)
                };

                var json = JsonConvert.SerializeObject(stats);
                _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.MovieGeneral, collectionIds);
            }

            return stats;
        }

        public async Task<PersonStats> GetPeopleStatsForCollections(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.MoviePeople);

            PersonStats stats;
            if (NewStatisticsNeeded(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<PersonStats>(statistic.JsonResult);
            }
            else
            {
                stats = new PersonStats
                {
                    TotalActorCount = TotalTypeCount(collectionIds, Constants.Actor, Constants.Common.TotalActors),
                    TotalDirectorCount = TotalTypeCount(collectionIds, Constants.Director, Constants.Common.TotalDirectors),
                    TotalWriterCount = TotalTypeCount(collectionIds, Constants.Writer, Constants.Common.TotalWriters),
                    MostFeaturedActor = await GetMostFeaturedPerson(collectionIds, Constants.Actor, Constants.Common.MostFeaturedActor),
                    MostFeaturedDirector = await GetMostFeaturedPerson(collectionIds, Constants.Director, Constants.Common.MostFeaturedDirector),
                    MostFeaturedWriter = await GetMostFeaturedPerson(collectionIds, Constants.Writer, Constants.Common.MostFeaturedWriter),
                    MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenre(collectionIds)
                };

                var json = JsonConvert.SerializeObject(stats);
                _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.MoviePeople, collectionIds);
            }

            return stats;
        }

        public MovieGraphs GetGraphs(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.MovieGraphs);

            MovieGraphs stats;
            if (NewStatisticsNeeded(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<MovieGraphs>(statistic.JsonResult);
            }
            else
            {
                var movies = _movieRepository.GetAll(collectionIds, true);

                stats = new MovieGraphs();
                stats.BarGraphs.Add(CalculateGenreGraph(movies));
                stats.BarGraphs.Add(CalculateRatingGraph(movies.Select(x => x.CommunityRating)));
                stats.BarGraphs.Add(CalculatePremiereYearGraph(movies.Select(x => x.PremiereDate)));
                stats.BarGraphs.Add(CalculateOfficialRatingGraph(movies));

                var json = JsonConvert.SerializeObject(stats);
                _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.MovieGraphs, collectionIds);
            }

            return stats;
        }

        public SuspiciousTables GetSuspiciousMovies(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.MovieSuspicious);

            SuspiciousTables stats;
            if (NewStatisticsNeeded(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<SuspiciousTables>(statistic.JsonResult);
            }
            else
            {
                var movies = _movieRepository.GetAll(collectionIds, true);
                stats = new SuspiciousTables
                {
                    Duplicates = GetDuplicates(movies),
                    Shorts = GetShortMovies(movies)
                };

                var json = JsonConvert.SerializeObject(stats);
                _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.MovieSuspicious, collectionIds);
            }

            return stats;
        }

        public bool MovieTypeIsPresent()
        {
            return _movieRepository.Any();
        }

        private List<ShortMovie> GetShortMovies(List<Movie> movies)
        {
            var configuration = _configurationRepository.GetConfiguration();
            var shortMovies = movies
                .Where(x => x.RunTimeTicks != null)
                .Where(x => new TimeSpan(x.RunTimeTicks ?? 0).TotalMinutes < configuration.ToShortMovie)
                .OrderBy(x => x.SortName)
                .Select((t, i) => new ShortMovie
                {
                    Number = i++,
                    Duration = Math.Floor(new TimeSpan(t.RunTimeTicks ?? 0).TotalMinutes),
                    Title = t.Name,
                    MediaId = t.Id
                })
                .ToList();

            return shortMovies;
        }

        private List<MovieDuplicate> GetDuplicates(List<Movie> movies)
        {
            var list = new List<MovieDuplicate>();

            var duplicatesByImdb = movies.Where(x => !string.IsNullOrWhiteSpace(x.IMDB)).GroupBy(x => x.IMDB).Select(x => new { x.Key, Count = x.Count() }).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByImdb.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.IMDB == duplicatesByImdb[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies.First();
                var itemTwo = duplicateMovies.ElementAt(1);

                list.Add(new MovieDuplicate
                {
                    Number = i,
                    Title = itemOne.Name,
                    Reason = Constants.ByImdb,
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) },
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) }
                });
            }

            var duplicateIds = list.Select(x => x.ItemOne.Id).ToList();
            duplicateIds.AddRange(list.Select(x => x.ItemTwo.Id).ToList());

            var duplicatesByName = movies
                .Where(x => duplicateIds.All(y => y != x.Id))
                .GroupBy(x => x.Name).Select(x => new { x.Key, Count = x.Count() }).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByName.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.Name == duplicatesByName[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies.First();
                var itemTwo = duplicateMovies.ElementAt(1);

                list.Add(new MovieDuplicate
                {
                    Number = i,
                    Title = itemOne.Name,
                    Reason = Constants.ByTitle,
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) },
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) }
                });
            }

            return list.OrderBy(x => x.Title).ToList();
        }

        #region StatCreators

        private Card TotalMovieCount(IEnumerable<Movie> movies)
        {
            return new Card
            {
                Title = Constants.Movies.TotalMovies,
                Value = movies.Count().ToString()
            };
        }

        private Card TotalMovieGenres(IEnumerable<Movie> movies)
        {
            return new Card
            {
                Title = Constants.Movies.TotalGenres,
                Value = movies.SelectMany(x => x.MediaGenres)
                              .Select(x => x.GenreId)
                              .Distinct()
                              .Count()
                              .ToString()
            };
        }

        private MoviePoster HighestRatedMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.CommunityRating != null)
                              .OrderByDescending(x => x.CommunityRating).ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.HighestRated);
            }

            return new MoviePoster();
        }

        private MoviePoster LowestRatedMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.CommunityRating != null)
                              .OrderBy(x => x.CommunityRating)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.LowestRated);
            }

            return new MoviePoster();
        }

        private MoviePoster OldestPremieredMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.PremiereDate != null)
                              .OrderBy(x => x.PremiereDate)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.OldestPremiered);
            }

            return new MoviePoster();
        }

        private MoviePoster YoungestPremieredMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.PremiereDate != null)
                              .OrderByDescending(x => x.PremiereDate)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.YoungestPremiered);
            }

            return new MoviePoster();
        }

        private MoviePoster ShortestMovie(IEnumerable<Movie> movies)
        {
            var configuration = _configurationRepository.GetConfiguration();
            var movie = movies.Where(x => x.RunTimeTicks != null && x.RunTimeTicks >= configuration.ToShortMovie)
                              .OrderBy(x => x.RunTimeTicks)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Shortest);
            }

            return new MoviePoster();
        }

        private MoviePoster LongestMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.RunTimeTicks != null)
                              .OrderByDescending(x => x.RunTimeTicks)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Longest);
            }

            return new MoviePoster();
        }

        private MoviePoster YoungestAddedMovie(IEnumerable<Movie> movies)
        {
            var movie = movies.Where(x => x.DateCreated != null)
                              .OrderByDescending(x => x.DateCreated)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.YoungestAdded);
            }

            return new MoviePoster();
        }

        private TimeSpanCard TotalPlayLength(IEnumerable<Movie> movies)
        {
            var playLength = new TimeSpan(movies.Sum(x => x.RunTimeTicks ?? 0));
            return new TimeSpanCard
            {
                Title = Constants.Movies.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }

        private Card TotalTypeCount(List<string> collectionsIds, string type, string title)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalPersonByType(collectionsIds, type).ToString(),
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPerson(List<string> collectionIds, string type, string title)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, type);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, title);
        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenre(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds, true);
            var genreIds = _movieRepository.GetGenres(collectionIds);
            var genres = _genreRepository.GetListByIds(genreIds);

            var list = new List<PersonPoster>();
            foreach (var genre in genres.OrderBy(x => x.Name))
            {
                var selectedMovies = movies.Where(x => x.MediaGenres.Any(y => y.GenreId == genre.Id));
                var personId = selectedMovies
                    .SelectMany(x => x.ExtraPersons)
                    .Where(x => x.Type == Constants.Actor)
                    .GroupBy(x => x.PersonId)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var person = await _personService.GetPersonById(personId);
                list.Add(PosterHelper.ConvertToPersonPoster(person, genre.Name));
            }

            return list;
        }

        private Graph<SimpleGraphValue> CalculateGenreGraph(List<Movie> movies)
        {
            var genres = _genreRepository.GetAll();
            var genresData = movies.SelectMany(x => x.MediaGenres).GroupBy(x => x.GenreId)
                .Select(x => new { Name = genres.Single(y => y.Id == x.Key).Name, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerGenre,
                Data = genresData
            };
        }

        private Graph<SimpleGraphValue> CalculateOfficialRatingGraph(List<Movie> movies)
        {
            var ratingData = movies
                .Where(x => !string.IsNullOrWhiteSpace(x.OfficialRating))
                .GroupBy(x => x.OfficialRating.ToUpper())
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerOfficialRating,
                Data = ratingData
            };
        }


        #endregion
    }
}
