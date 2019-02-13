using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;

namespace EmbyStat.Services
{
    public class MovieService : MediaService, IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonService _personService;
        private readonly ISettingsService _settingsService;
        private readonly IStatisticsRepository _statisticsRepository;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, 
            IGenreRepository genreRepository, IPersonService personService, ISettingsService settingsService, 
            IStatisticsRepository statisticsRepository, IJobRepository jobRepository): base(jobRepository)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
        }

        public IEnumerable<Collection> GetMovieCollections()
        {
            var settings = _settingsService.GetUserSettings();
            return _collectionRepository.GetCollectionByTypes(settings.MovieCollectionTypes);
        }

        public MovieStats GetGeneralStatsForCollections(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.MovieGeneral);

            MovieStats stats;
            if (StatisticsAreValid(statistic, collectionIds))
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
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<PersonStats>(statistic.JsonResult);
            }
            else
            {
                stats = new PersonStats
                {
                    TotalActorCount = TotalTypeCount(collectionIds, PersonType.Actor, Constants.Common.TotalActors),
                    TotalDirectorCount = TotalTypeCount(collectionIds, PersonType.Director, Constants.Common.TotalDirectors),
                    TotalWriterCount = TotalTypeCount(collectionIds, PersonType.Writer, Constants.Common.TotalWriters),
                    MostFeaturedActor = await GetMostFeaturedPerson(collectionIds, PersonType.Actor, Constants.Common.MostFeaturedActor),
                    MostFeaturedDirector = await GetMostFeaturedPerson(collectionIds, PersonType.Director, Constants.Common.MostFeaturedDirector),
                    MostFeaturedWriter = await GetMostFeaturedPerson(collectionIds, PersonType.Writer, Constants.Common.MostFeaturedWriter),
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
            if (StatisticsAreValid(statistic, collectionIds))
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
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<SuspiciousTables>(statistic.JsonResult);
            }
            else
            {
                var movies = _movieRepository.GetAll(collectionIds, true);
                stats = new SuspiciousTables
                {
                    Duplicates = GetDuplicates(movies),
                    Shorts = GetShortMovies(movies),
                    NoImdb = GetMoviesWithoutImdbLink(movies),
                    NoPrimary = GetMoviesWithoutPrimaryImage(movies)
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

        private List<ShortMovie> GetShortMovies(IEnumerable<Movie> movies)
        {
            var settings = _settingsService.GetUserSettings();
            var shortMovies = movies
                .Where(x => x.RunTimeTicks != null)
                .Where(x => new TimeSpan(x.RunTimeTicks ?? 0).TotalMinutes < settings.ToShortMovie)
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

        private List<SuspiciousMovie> GetMoviesWithoutImdbLink(IEnumerable<Movie> movies)
        {
            var noImdbMovies = movies
                .Where(x => string.IsNullOrWhiteSpace(x.IMDB))
                .OrderBy(x => x.SortName)
                .Select((t, i) => new SuspiciousMovie
                {
                    Number = i++,
                    Title = t.Name,
                    MediaId = t.Id
                })
                .ToList();

            return noImdbMovies;
        }

        private List<SuspiciousMovie> GetMoviesWithoutPrimaryImage(IEnumerable<Movie> movies)
        {
            var noPrimaryImageMovies = movies
                .Where(x => string.IsNullOrWhiteSpace(x.Primary))
                .OrderBy(x => x.SortName)
                .Select((t, i) => new SuspiciousMovie
                {
                    Number = i++,
                    Title = t.Name,
                    MediaId = t.Id
                })
                .ToList();

            return noPrimaryImageMovies;
        }

        private List<MovieDuplicate> GetDuplicates(IReadOnlyCollection<Movie> movies)
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
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityConverter.ConvertToQualityString(x.Width))) },
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityConverter.ConvertToQualityString(x.Width))) }
                });
            }

            return list.OrderBy(x => x.Title).ToList();
        }

        #region StatCreators

        private Card<int> TotalMovieCount(IEnumerable<Movie> movies)
        {
            return new Card<int>
            {
                Title = Constants.Movies.TotalMovies,
                Value = movies.Count()
            };
        }

        private Card<int> TotalMovieGenres(IEnumerable<Movie> movies)
        {
            return new Card<int>
            {
                Title = Constants.Movies.TotalGenres,
                Value = movies.SelectMany(x => x.MediaGenres)
                              .Select(x => x.GenreId)
                              .Distinct()
                              .Count()
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
            var settings = _settingsService.GetUserSettings();
            var movie = movies.Where(x => x.RunTimeTicks != null && x.RunTimeTicks >= settings.ToShortMovie)
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

        private Card<int> TotalTypeCount(IEnumerable<string> collectionsIds, string type, string title)
        {
            return new Card<int>
            {
                Value = _movieRepository.GetTotalPersonByType(collectionsIds, type),
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPerson(IEnumerable<string> collectionIds, string type, string title)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, type);

            var person = await _personService.GetPersonById(personId);
            person.MovieCount = _movieRepository.GetMovieCountForPerson(personId);
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
                    .Where(x => x.Type == PersonType.Actor)
                    .GroupBy(x => x.PersonId)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var person = await _personService.GetPersonById(personId);
                person.MovieCount = _movieRepository.GetMovieCountForPerson(personId);
                list.Add(PosterHelper.ConvertToPersonPoster(person, genre.Name));
            }

            return list;
        }

        private Graph<SimpleGraphValue> CalculateGenreGraph(IEnumerable<Movie> movies)
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

        private Graph<SimpleGraphValue> CalculateOfficialRatingGraph(IEnumerable<Movie> movies)
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
