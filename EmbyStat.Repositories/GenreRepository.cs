using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly LiteCollection<Genre> _genreCollection;

        public GenreRepository(IDbContext context)
        {
            _genreCollection = context.GetContext().GetCollection<Genre>();
        }

        public void UpsertRange(IEnumerable<Genre> genres)
        {
            _genreCollection.Upsert(genres);
        }

        public IEnumerable<Genre> GetAll()
        {
            return _genreCollection.FindAll();
        }

        public IEnumerable<string> GetIds()
        {
            return _genreCollection.FindAll().Select(x => x.Id);
        }

        public IEnumerable<Genre> GetGenres(IEnumerable<string> ids)
        {
            var bArray = new BsonArray();
            foreach (var id in ids)
            {
                bArray.Add(id);
            }

            return _genreCollection.Find(Query.In("Id", bArray));
        }

        public void CleanupGenres()
        {
            throw new NotImplementedException();
        }
    }
}
