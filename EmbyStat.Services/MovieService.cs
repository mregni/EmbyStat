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
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using Newtonsoft.Json;

namespace EmbyStat.Services
{
    public class MovieService : MediaService, IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IPersonService _personService;
        private readonly ISettingsService _settingsService;
        private readonly IStatisticsRepository _statisticsRepository;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository,
            IPersonService personService, ISettingsService settingsService,
            IStatisticsRepository statisticsRepository, IJobRepository jobRepository) : base(jobRepository)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _personService = personService;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
        }

        public IEnumerable<Collection> GetMovieCollections()
        {
            var settings = _settingsService.GetUserSettings();
            return _collectionRepository.GetCollectionByTypes(settings.MovieCollectionTypes);
        }

        public async Task<MovieStatistics> GetMovieStatisticsAsync(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Movie, collectionIds);

            MovieStatistics statistics;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                statistics = JsonConvert.DeserializeObject<MovieStatistics>(statistic.JsonResult);
            }
            else
            {
                statistics = await CalculateMovieStatistics(collectionIds);
            }

            return statistics;
        }

        public async Task<MovieStatistics> CalculateMovieStatistics(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds).ToList();

            var statistics = new MovieStatistics
            {
                General = CalculateGeneralStatistics(movies),
                Charts = CalculateCharts(movies),
                People = await CalculatePeopleStatistics(movies),
                Suspicious = CalculateSuspiciousMovies(movies)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Movie, collectionIds);

            return statistics;
        }

        private MovieGeneral CalculateGeneralStatistics(IReadOnlyCollection<Movie> movies)
        {
            return new MovieGeneral
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
        }

        public async Task<PersonStats> CalculatePeopleStatistics(IReadOnlyCollection<Movie> movies)
        {
            return new PersonStats
            {
                TotalActorCount = TotalTypeCount(movies, PersonType.Actor, Constants.Common.TotalActors),
                TotalDirectorCount = TotalTypeCount(movies, PersonType.Director, Constants.Common.TotalDirectors),
                TotalWriterCount = TotalTypeCount(movies, PersonType.Writer, Constants.Common.TotalWriters),
                MostFeaturedActor = await GetMostFeaturedPersonAsync(movies, PersonType.Actor, Constants.Common.MostFeaturedActor),
                MostFeaturedDirector = await GetMostFeaturedPersonAsync(movies, PersonType.Director, Constants.Common.MostFeaturedDirector),
                MostFeaturedWriter = await GetMostFeaturedPersonAsync(movies, PersonType.Writer, Constants.Common.MostFeaturedWriter),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenreAsync(movies)
            };
        }

        private MovieCharts CalculateCharts(IReadOnlyCollection<Movie> movies)
        {
            var stats = new MovieCharts();
            stats.BarCharts.Add(CalculateGenreChart(movies));
            stats.BarCharts.Add(CalculateRatingChart(movies.Select(x => x.CommunityRating)));
            stats.BarCharts.Add(CalculatePremiereYearChart(movies.Select(x => x.PremiereDate)));
            stats.BarCharts.Add(CalculateOfficialRatingChart(movies));

            return stats;
        }

        public SuspiciousTables CalculateSuspiciousMovies(IReadOnlyCollection<Movie> movies)
        {
            return new SuspiciousTables
            {
                Duplicates = GetDuplicates(movies),
                Shorts = GetShortMovies(movies),
                NoImdb = GetMoviesWithoutImdbLink(movies),
                NoPrimary = GetMoviesWithoutPrimaryImage(movies)
            };
        }

        public bool TypeIsPresent()
        {
            return _movieRepository.Any();
        }

        private IEnumerable<ShortMovie> GetShortMovies(IEnumerable<Movie> movies)
        {
            var settings = _settingsService.GetUserSettings();
            var shortMovies = movies
                .Where(x => x.RunTimeTicks != null)
                .Where(x => new TimeSpan(x.RunTimeTicks ?? 0).TotalMinutes < settings.ToShortMovie)
                .OrderBy(x => x.SortName);
            return shortMovies.Select((t, i) => new ShortMovie
            {
                Number = i++,
                Duration = Math.Floor(new TimeSpan(t.RunTimeTicks ?? 0).TotalMinutes),
                Title = t.Name,
                MediaId = t.Id
            }).ToList();
        }

        private IEnumerable<SuspiciousMovie> GetMoviesWithoutImdbLink(IEnumerable<Movie> movies)
        {
            var noImdbMovies = movies
                .Where(x => string.IsNullOrWhiteSpace(x.IMDB))
                .OrderBy(x => x.SortName)
                .Select((t, i) => new SuspiciousMovie
                {
                    Number = i++,
                    Title = t.Name,
                    MediaId = t.Id
                });

            return noImdbMovies;
        }

        private IEnumerable<SuspiciousMovie> GetMoviesWithoutPrimaryImage(IEnumerable<Movie> movies)
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

        private IEnumerable<MovieDuplicate> GetDuplicates(IReadOnlyCollection<Movie> movies)
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
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = string.Join(",", itemOne.VideoStreams.Select(x => QualityConverter.ConvertToQualityString(x.Width))) },
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = string.Join(",", itemTwo.VideoStreams.Select(x => QualityConverter.ConvertToQualityString(x.Width))) }
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
                Value = movies.SelectMany(x => x.Genres)
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

        private Card<int> TotalTypeCount(IEnumerable<Movie> movies, string type, string title)
        {
            var value = movies.SelectMany(x => x.People)
                .DistinctBy(x => x.Id)
                .Count(x => x.Type == type);
            return new Card<int>
            {
                Value = value,
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPersonAsync(IEnumerable<Movie> movies, string type, string title)
        {
            var personId = movies.SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Id, x => x.Name, (id, name) => new { Key = id, Count = name.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Key)
                .FirstOrDefault();
            if (personId != null)
            {
                var person = await _personService.GetPersonByIdAsync(personId);
                if (person != null)
                {
                    return PosterHelper.ConvertToPersonPoster(person, title);
                }
            }

            return new PersonPoster(title);

        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenreAsync(IReadOnlyCollection<Movie> movies)
        {
            var list = new List<PersonPoster>();
            foreach (var genre in movies.SelectMany(x => x.Genres).Distinct().OrderBy(x => x))
            {
                var selectedMovies = movies.Where(x => x.Genres.Any(y => y == genre));
                var personId = selectedMovies
                    .SelectMany(x => x.People)
                    .Where(x => x.Type == PersonType.Actor)
                    .GroupBy(x => x.Id)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var person = await _personService.GetPersonByIdAsync(personId);
                if (person != null)
                {
                    list.Add(PosterHelper.ConvertToPersonPoster(person, genre));
                }
            }

            return list;
        }

        private Chart CalculateGenreChart(IEnumerable<Movie> movies)
        {
            var genresData = movies
                .SelectMany(x => x.Genres)
                .GroupBy(x => x)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerGenre,
                Labels = genresData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { genresData.Select(x => x.Count) }
            };
        }

        private Chart CalculateOfficialRatingChart(IEnumerable<Movie> movies)
        {
            var ratingData = movies
                .Where(x => !string.IsNullOrWhiteSpace(x.OfficialRating))
                .GroupBy(x => x.OfficialRating.ToUpper())
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerOfficialRating,
                Labels = ratingData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { ratingData.Select(x => x.Count) }
            };
        }
        #endregion
    }
}
