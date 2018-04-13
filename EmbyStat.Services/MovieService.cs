using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
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
    }
}
