using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LiteCollection<Library> _libraryCollection;

        public LibraryRepository(IDbContext context)
        {
            _libraryCollection = context.GetContext().GetCollection<Library>();
        }
        public IEnumerable<Library> GetLibrariesByTypes(IEnumerable<CollectionType> types)
        {
            var bArray = new BsonArray();
            foreach (var type in types)
            {
                bArray.Add(type.ToString()); 
            }
            return _libraryCollection.Find(Query.In("Type", bArray));
        }

        public void AddOrUpdateRange(IEnumerable<Library> collections)
        {
            _libraryCollection.Upsert(collections);
        }
    }
}
