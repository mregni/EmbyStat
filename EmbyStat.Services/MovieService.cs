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
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IEmbyClient _embyClient;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, IConfigurationRepository configurationRepository, IEmbyClient embyClient)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _configurationRepository = configurationRepository;
            _embyClient = embyClient;
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
                MostFeaturedActor = await GetMostFeaturedActor(collectionsIds)
            };
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
            var actorId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Actor);
            var settings = _configurationRepository.GetSingle();
            var query = new ItemQuery {UserId = settings.EmbyUserId};

            _embyClient.SetAddressAndUrl(settings.EmbyServerAddress, settings.AccessToken);
            var person = await _embyClient.GetItemAsync(query, actorId, CancellationToken.None);
            return new PersonPoster
            {
                Id = person.Id,
                Name = person.Name,
                BirthDate = person.PremiereDate,
                ChildCount = person.ChildCount,
                MovieCount = person.MovieCount,
                EpisodeCount = person.EpisodeCount,
                Title = Constants.MoviesMostFeaturedActor,
                HasTitle = true,
                Tag = person.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value
            };
        }
    }
#endregion
}
