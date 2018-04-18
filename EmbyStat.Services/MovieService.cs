using System;
using System.Collections.Generic;
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
            var playLength = new TimeSpan(_movieRepository.GetPlayLength(collectionIds));
            var playLengthCard = new TimeSpanCard
            {
                Title = "MOVIES.TOTALPLAYLENGTH",
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };

            var movieCountCard = new Card
            {
                Title = "MOVIES.TOTALMOVIES",
                Value = _movieRepository.GetMovieCount(collectionIds).ToString()
            };

            var genreCountCard = new Card
            {
                Title = "MOVIES.TOTALGENRES",
                Value = _movieRepository.GetGenreCount(collectionIds).ToString()
            };

            var result = new MovieStats
            {
                MovieCount = movieCountCard,
                GenreCount = genreCountCard,
                TotalPlayableTime = playLengthCard
            };

            return result;
        }
    }
}
