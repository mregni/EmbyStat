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
using Newtonsoft.Json;

namespace EmbyStat.Services
{
    public class MovieService : MediaService, IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ISettingsService _settingsService;
        private readonly IStatisticsRepository _statisticsRepository;

        public MovieService(IMovieRepository movieRepository, ILibraryRepository libraryRepository,
            IPersonService personService, ISettingsService settingsService,
            IStatisticsRepository statisticsRepository, IJobRepository jobRepository) : base(jobRepository, personService)
        {
            _movieRepository = movieRepository;
            _libraryRepository = libraryRepository;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
        }

        public IEnumerable<Library> GetMovieLibraries()
        {
            var settings = _settingsService.GetUserSettings();
            return _libraryRepository.GetLibrariesByTypes(settings.MovieLibraryTypes);
        }

        public async Task<MovieStatistics> GetStatisticsAsync(List<string> libraryIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Movie, libraryIds);

            MovieStatistics statistics;
            if (StatisticsAreValid(statistic, libraryIds))
            {
                statistics = JsonConvert.DeserializeObject<MovieStatistics>(statistic.JsonResult);

                if (!_settingsService.GetUserSettings().ToShortMovieEnabled && statistics.Suspicious.Shorts.Any())
                {
                    statistics.Suspicious.Shorts = new List<ShortMovie>();
                }
            }
            else
            {
                statistics = await CalculateMovieStatistics(libraryIds);
            }

            return statistics;
        }

        public async Task<MovieStatistics> CalculateMovieStatistics(List<string> libraryIds)
        {
            var statistics = new MovieStatistics
            {
                General = CalculateGeneralStatistics(libraryIds),
                Charts = CalculateCharts(libraryIds),
                People = await CalculatePeopleStatistics(libraryIds),
                Suspicious = CalculateSuspiciousMovies(libraryIds)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Movie, libraryIds);

            return statistics;
        }

        public bool TypeIsPresent()
        {
            return _movieRepository.Any();
        }

        #region General

        private MovieGeneral CalculateGeneralStatistics(IReadOnlyList<string> libraryIds)
        {
            var general = new MovieGeneral
            {
                MovieCount = TotalMovieCount(libraryIds),
                GenreCount = TotalMovieGenres(libraryIds),
                TotalPlayableTime = TotalPlayLength(libraryIds),
                TotalDiskSize = CalculateTotalDiskSize(libraryIds),
                HighestRatedMovie = HighestRatedMovie(libraryIds),
                LowestRatedMovie = LowestRatedMovie(libraryIds),
                OldestPremieredMovie = OldestPremieredMovie(libraryIds),
                NewestPremieredMovie = NewestPremieredMovie(libraryIds),
                ShortestMovie = ShortestMovie(libraryIds),
                LongestMovie = LongestMovie(libraryIds),
                LatestAddedMovie = LatestAddedMovie(libraryIds)
            };

            return general;
        }

        private Card<int> TotalMovieCount(IReadOnlyList<string> libraryIds)
        {
            var count = _movieRepository.GetMediaCount(libraryIds);
            return new Card<int>
            {
                Title = Constants.Movies.TotalMovies,
                Value = count
            };
        }

        private Card<int> TotalMovieGenres(IReadOnlyList<string> libraryIds)
        {
            var totalGenres = _movieRepository.GetGenreCount(libraryIds);
            return new Card<int>
            {
                Title = Constants.Movies.TotalGenres,
                Value = totalGenres
            };
        }

        private MoviePoster HighestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            var highestRatingMovie = _movieRepository.GetHighestRatedMedia(libraryIds);

            return highestRatingMovie != null
                ? PosterHelper.ConvertToMoviePoster(highestRatingMovie, Constants.Movies.HighestRated)
                : new MoviePoster();
        }

        private MoviePoster LowestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            var lowestRatedMovie = _movieRepository.GetLowestRatedMedia(libraryIds);

            return lowestRatedMovie != null
                ? PosterHelper.ConvertToMoviePoster(lowestRatedMovie, Constants.Movies.LowestRated)
                : new MoviePoster();
        }

        private MoviePoster OldestPremieredMovie(IReadOnlyList<string> libraryIds)
        {
            var movie = _movieRepository.GetOldestPremieredMedia(libraryIds);

            return movie != null
                ? PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.OldestPremiered)
                : new MoviePoster();
        }

        private MoviePoster NewestPremieredMovie(IReadOnlyList<string> libraryIds)
        {
            var movie = _movieRepository.GetNewestPremieredMedia(libraryIds);

            return movie != null
                ? PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.NewestPremiered)
                : new MoviePoster();
        }

        private MoviePoster ShortestMovie(IReadOnlyList<string> libraryIds)
        {
            var settings = _settingsService.GetUserSettings();
            var toShortMovieTicks = TimeSpan.FromMinutes(settings.ToShortMovie).Ticks;
            var movie = _movieRepository.GetShortestMovie(libraryIds, toShortMovieTicks);

            return movie != null
                ? PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Shortest)
                : new MoviePoster();
        }

        private MoviePoster LongestMovie(IReadOnlyList<string> libraryIds)
        {
            var movie = _movieRepository.GetLongestMovie(libraryIds);

            return movie != null
                ? PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Longest) :
                new MoviePoster();
        }

        private MoviePoster LatestAddedMovie(IReadOnlyList<string> libraryIds)
        {
            var movie = _movieRepository.GetLatestAddedMedia(libraryIds);

            return movie != null
                ? PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.LatestAdded) :
                new MoviePoster();
        }

        private TimeSpanCard TotalPlayLength(IReadOnlyList<string> libraryIds)
        {
            var playLengthTicks = _movieRepository.GetTotalRuntime(libraryIds);
            var playLength = new TimeSpan(playLengthTicks);

            return new TimeSpanCard
            {
                Title = Constants.Movies.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }

        protected Card<double> CalculateTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            var sum = _movieRepository.GetTotalDiskSize(libraryIds);
            return new Card<double>
            {
                Value = sum,
                Title = Constants.Common.TotalDiskSize
            };
        }

        #endregion

        #region Charts

        private MovieCharts CalculateCharts(IReadOnlyList<string> libraryIds)
        {
            var movies = _movieRepository.GetAll(libraryIds);
            var stats = new MovieCharts();
            stats.BarCharts.Add(CalculateGenreChart(movies));
            stats.BarCharts.Add(CalculateRatingChart(movies.Select(x => x.CommunityRating)));
            stats.BarCharts.Add(CalculatePremiereYearChart(movies.Select(x => x.PremiereDate)));
            stats.BarCharts.Add(CalculateOfficialRatingChart(movies));

            return stats;
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

        #region People

        public async Task<PersonStats> CalculatePeopleStatistics(IReadOnlyList<string> libraryIds)
        {
            return new PersonStats
            {
                TotalActorCount = TotalTypeCount(libraryIds, PersonType.Actor, Constants.Common.TotalActors),
                TotalDirectorCount = TotalTypeCount(libraryIds, PersonType.Director, Constants.Common.TotalDirectors),
                TotalWriterCount = TotalTypeCount(libraryIds, PersonType.Writer, Constants.Common.TotalWriters),
                MostFeaturedActor = await GetMostFeaturedPersonAsync(libraryIds, PersonType.Actor, Constants.Common.MostFeaturedActor),
                MostFeaturedDirector = await GetMostFeaturedPersonAsync(libraryIds, PersonType.Director, Constants.Common.MostFeaturedDirector),
                MostFeaturedWriter = await GetMostFeaturedPersonAsync(libraryIds, PersonType.Writer, Constants.Common.MostFeaturedWriter),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenreAsync(libraryIds)
            };
        }

        private Card<int> TotalTypeCount(IReadOnlyList<string> libraryIds, PersonType type, string title)
        {
            var value = _movieRepository.GetPeopleCount(libraryIds, type);
            return new Card<int>
            {
                Value = value,
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPersonAsync(IReadOnlyList<string> libraryIds, PersonType type, string title)
        {
            var personName = _movieRepository.GetMostFeaturedPerson(libraryIds, type);
            if (personName != null)
            {
                var person = await PersonService.GetPersonByNameAsync(personName);
                if (person != null)
                {
                    return PosterHelper.ConvertToPersonPoster(person, title);
                }
            }

            return new PersonPoster(title);

        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenreAsync(IReadOnlyList<string> libraryIds)
        {
            var movies = _movieRepository.GetAll(libraryIds);
            return await GetMostFeaturedActorsPerGenreAsync(movies);
        }

        #endregion

        #region Suspicious

        public SuspiciousTables CalculateSuspiciousMovies(IReadOnlyList<string> libraryIds)
        {
            return new SuspiciousTables
            {
                Duplicates = GetDuplicates(libraryIds),
                Shorts = GetShortMovies(libraryIds),
                NoImdb = GetMoviesWithoutImdbLink(libraryIds),
                NoPrimary = GetMoviesWithoutPrimaryImage(libraryIds)
            };
        }

        private IEnumerable<ShortMovie> GetShortMovies(IReadOnlyList<string> libraryIds)
        {
            var settings = _settingsService.GetUserSettings();
            if (!settings.ToShortMovieEnabled)
            {
                return new List<ShortMovie>(0);
            }

            var shortMovies = _movieRepository.GetToShortMovieList(libraryIds, settings.ToShortMovie);
            return shortMovies.Select((t, i) => new ShortMovie
            {
                Number = i,
                Duration = Math.Floor(new TimeSpan(t.RunTimeTicks ?? 0).TotalMinutes),
                Title = t.Name,
                MediaId = t.Id
            }).ToList();
        }

        private IEnumerable<SuspiciousMovie> GetMoviesWithoutImdbLink(IReadOnlyList<string> libraryIds)
        {
            var moviesWithoutImdbId = _movieRepository.GetMoviesWithoutImdbId(libraryIds);
            return moviesWithoutImdbId
                .Select((t, i) => new SuspiciousMovie
                {
                    Number = i,
                    Title = t.Name,
                    MediaId = t.Id
                });
        }

        private IEnumerable<SuspiciousMovie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
        {
            var noPrimaryImageMovies = _movieRepository.GetMoviesWithoutPrimaryImage(libraryIds);
            return noPrimaryImageMovies.Select((t, i) => new SuspiciousMovie
            {
                Number = i,
                Title = t.Name,
                MediaId = t.Id
            })
            .ToList();
        }

        private IEnumerable<MovieDuplicate> GetDuplicates(IReadOnlyList<string> libraryIds)
        {
            var list = new List<MovieDuplicate>();

            var movies = _movieRepository.GetAllWithImdbId(libraryIds);
            var duplicatesByImdb = movies
                .GroupBy(x => x.IMDB)
                .Select(x => new { x.Key, Count = x.Count() })
                .Where(x => x.Count > 1)
                .ToList();

            for (var i = 0; i < duplicatesByImdb.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.IMDB == duplicatesByImdb[i].Key).OrderBy(x => x.SortName).ToList();
                var itemOne = duplicateMovies[0];
                var itemTwo = duplicateMovies[1];

                if (itemOne.Video3DFormat != itemTwo.Video3DFormat)
                {
                    continue;
                }

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

        #endregion
    }
}
