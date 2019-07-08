using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;
using NLog;
using Logger = NLog.Logger;

namespace EmbyStat.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly LiteCollection<Movie> _movieCollection;

        private readonly Logger _logger;

        public MovieRepository(IDbContext context)
        {
            _logger = LogManager.GetCurrentClassLogger();

            _movieCollection = context.GetContext().GetCollection<Movie>();
        }

        public void UpsertRange(IEnumerable<Movie> movies)
        {
            _movieCollection.Upsert(movies);
        }

        public int GetTotalPeopleByType(IEnumerable<string> collectionIds, string type)
        {
            var movies = collectionIds.Any() ?
                _movieCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _movieCollection.FindAll();

            return movies
                .SelectMany(x => x.People)
                .DistinctBy(x => x.Id)
                .Count(x => x.Type == type);
        }

        public string GetMostFeaturedPerson(IEnumerable<string> collectionIds, string type)
        {
            var shows = collectionIds.Any() ?
                _movieCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _movieCollection.FindAll();

            return shows
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Id, x => x.Name, (id, name) => new { Key = id, Count = name.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        public IEnumerable<Movie> GetAll(IEnumerable<string> collectionIds)
        {
            return collectionIds.Any() ?
                _movieCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _movieCollection.FindAll();
        }

        public IEnumerable<string> GetGenres(IEnumerable<string> collectionIds)
        {
            var movies = collectionIds.Any() ?
                _movieCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _movieCollection.FindAll();

            return movies
                .SelectMany(x => x.GenresIds)
                .Distinct();
        }

        public bool Any()
        {
            return _movieCollection.Exists(Query.All());
        }

        public int GetMovieCountForPerson(string personId)
        {
            return _movieCollection.Count(Query.EQ("People[*].Id", personId));
        }

        public Movie GetMovieById(string id)
        {
            return _movieCollection.FindById(id);
        }

        public void RemoveMovies()
        {
            _movieCollection.Delete(Query.All());
        }
    }
}
