using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using CollectionType = EmbyStat.Common.Models.CollectionType;
using Constants = EmbyStat.Common.Constants;

namespace EmbyStat.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonService _personService;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, IGenreRepository genreRepository, IPersonService personService)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
        }

        public List<Collection> GetMovieCollections()
        {
            return _collectionRepository.GetCollectionByType(CollectionType.Movies).ToList();
        }

        public MovieStats GetGeneralStatsForCollections(List<string> collectionIds)
        {
            return new MovieStats
            {
                MovieCount = TotalMovieCount(collectionIds),
                GenreCount = TotalMovieGenres(collectionIds),
                TotalPlayableTime = TotalPlayLength(collectionIds),
                HighestRatedMovie = HighestRatedMovie(collectionIds),
                LowestRatedMovie = LowestRatedMovie(collectionIds),
                OldestPremieredMovie = OldestPremieredMovie(collectionIds),
                YoungestPremieredMovie = YoungestPremieredMovie(collectionIds),
                ShortestMovie = ShortestMovie(collectionIds),
                LongestMovie = LongestMovie(collectionIds),
                YoungestAddedMovie = YoungestAddedMovie(collectionIds)
            };
        }

        public async Task<MoviePersonStats> GetPeopleStatsForCollections(List<string> collectionsIds)
        {
            return new MoviePersonStats
            {
                TotalActorCount = TotalActorCount(collectionsIds),
                TotalDirectorCount = TotalDirectorCount(collectionsIds),
                TotalWriterCount = TotalWriterCount(collectionsIds),
                MostFeaturedActor = await GetMostFeaturedActor(collectionsIds),
                MostFeaturedDirector = await GetMostFeaturedDirector(collectionsIds),
                MostFeaturedWriter = await GetMostFeaturedWriter(collectionsIds),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenre(collectionsIds)
            };
        }

        public List<MovieDuplicate> GetDuplicates(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);
            var list = new List<MovieDuplicate>();

            var duplicatesByName = movies.GroupBy(x => x.Name).Select(x => new {x.Key, Count = x.Count()}).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByName.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.Name == duplicatesByName[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies.First();
                var itemTwo = duplicateMovies.Skip(1).First();

                list.Add(new MovieDuplicate
                {
                    Number = i,
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Title = itemOne.Name, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width)))},
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Title = itemTwo.Name, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width)))}
                });
            }


            //var duplicatesByImdb = movies.GroupBy(x => x.IMDB).Select(x => new {x.Key, Count = x.Count()}).Where(x => x.Count > 1).ToList();

            return list;
        }

        #region StatCreators

        private Card TotalMovieCount(List<string> collectionIds)
        {
            return new Card
            {
                Title = Constants.MoviesTotalMovies,
                Value = _movieRepository.GetMovieCount(collectionIds).ToString()
            };
        }

        private Card TotalMovieGenres(List<string> collectionIds)
        {
            return new Card
            {
                Title = Constants.MoviesTotalGenres,
                Value = _movieRepository.GetGenreCount(collectionIds).ToString()
            };
        }

        private Poster HighestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetHighestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesHighestRated);
            }

            return new Poster();
        }

        private Poster LowestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLowestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesLowestRated);
            }

            return new Poster();
        }

        private Poster OldestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetOlderPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesOldestPremiered);
            }

            return new Poster();
        }

        private Poster YoungestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesYoungestPremiered);
            }

            return new Poster();
        }

        private Poster ShortestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetShortestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesShortest);
            }

            return new Poster();
        }

        private Poster LongestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLongestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesLongest);
            }

            return new Poster();
        }

        private Poster YoungestAddedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestAddedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.MoviesYoungestAdded);
            }

            return new Poster();
        }

        private TimeSpanCard TotalPlayLength(List<string> collectionIds)
        {
            var playLength = new TimeSpan(_movieRepository.GetPlayLength(collectionIds));
            return new TimeSpanCard
            {
                Title = Constants.MoviesTotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }

        private Card TotalActorCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalActors(collectionsIds).ToString(),
                Title = Constants.MoviesTotalActors
            };
        }

        private Card TotalDirectorCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalDirectors(collectionsIds).ToString(),
                Title = Constants.MoviesTotalDirectors
            };
        }

        private Card TotalWriterCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalWriters(collectionsIds).ToString(),
                Title = Constants.MoviesTotalWriters
            };
        }

        private async Task<PersonPoster> GetMostFeaturedActor(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Actor);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.MoviesMostFeaturedActor);
        }

        private async Task<PersonPoster> GetMostFeaturedDirector(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Director);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.MoviesMostFeaturedDirector);
        }

        private async Task<PersonPoster> GetMostFeaturedWriter(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Writer);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.MoviesMostFeaturedWriter);
        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenre(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);
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
    }
#endregion
}
