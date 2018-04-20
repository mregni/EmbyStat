using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Stats;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
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
                YoungestPremieredMovie = YoungestPremieredMovie(collectionIds)
            };
        }


        private Card TotalMovieCount(List<string> collectionIds)
        {
            return new Card
            {
                Title = "MOVIES.TOTALMOVIES",
                Value = _movieRepository.GetMovieCount(collectionIds).ToString()
            };
        }
        private Card TotalMovieGenres(List<string> collectionIds)
        {
            return new Card
            {
                Title = "MOVIES.TOTALGENRES",
                Value = _movieRepository.GetGenreCount(collectionIds).ToString()
            };
        }
        private Poster HighestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetHighestRatedMovie(collectionIds);
            if (movie != null)
            {
                return new Poster
                {
                    Title = "MOVIES.HIGHESTRATED",
                    Name = movie.Name,
                    CommunityRating = Math.Floor(movie.CommunityRating ?? 0).ToString(CultureInfo.InvariantCulture),
                    MediaId = movie.Id,
                    OfficialRating = movie.OfficialRating,
                    Tag = movie.Primary
                };
            }

            return new Poster();
        }
        private Poster LowestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLowestRatedMovie(collectionIds);
            if (movie != null)
            {
                return new Poster
                {
                    Title = "MOVIES.LOWESTRATED",
                    Name = movie.Name,
                    CommunityRating = Math.Floor(movie.CommunityRating ?? 0).ToString(CultureInfo.InvariantCulture),
                    MediaId = movie.Id,
                    OfficialRating = movie.OfficialRating,
                    Tag = movie.Primary
                };
            }

            return new Poster();
        }
        private Poster OldestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetOlderPremieredMovie(collectionIds);
            if (movie != null)
            {
                return new Poster
                {
                    Title = "MOVIES.OLDESTPREMIEREDMOVIE",
                    Name = movie.Name,
                    CommunityRating = Math.Floor(movie.CommunityRating ?? 0).ToString(CultureInfo.InvariantCulture),
                    MediaId = movie.Id,
                    OfficialRating = movie.OfficialRating,
                    Tag = movie.Primary
                };
            }

            return new Poster();
        }
        private Poster YoungestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestPremieredMovie(collectionIds);
            if (movie != null)
            {
                return new Poster
                {
                    Title = "MOVIES.YOUNGESTPREMIEREDMOVIE",
                    Name = movie.Name,
                    CommunityRating = Math.Floor(movie.CommunityRating ?? 0).ToString(CultureInfo.InvariantCulture),
                    MediaId = movie.Id,
                    OfficialRating = movie.OfficialRating,
                    Tag = movie.Primary
                };
            }

            return new Poster();
        }
        private TimeSpanCard TotalPlayLength(List<string> collectionIds)
        {
            var playLength = new TimeSpan(_movieRepository.GetPlayLength(collectionIds));
            return new TimeSpanCard
            {
                Title = "MOVIES.TOTALPLAYLENGTH",
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
    }
    }
}
