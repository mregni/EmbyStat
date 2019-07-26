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

        public MovieRepository(IDbContext context)
        {
            _movieCollection = context.GetContext().GetCollection<Movie>();
        }

        public void UpsertRange(IEnumerable<Movie> movies)
        {
            _movieCollection.Upsert(movies);
        }

        public IEnumerable<Movie> GetAll(IEnumerable<string> collectionIds)
        {
            var bArray = new BsonArray();
            foreach (var collectionId in collectionIds)
            {
                bArray.Add(collectionId);
            }
            return collectionIds.Any() ?
                _movieCollection.Find(Query.In("CollectionId", bArray)) :
                _movieCollection.FindAll();
        }


        public bool Any()
        {
            return _movieCollection.Exists(Query.All());
        }

        public int GetMovieCountForPerson(string personId)
        {
            return _movieCollection.Count(Query.EQ("People[*]._id", personId));
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
