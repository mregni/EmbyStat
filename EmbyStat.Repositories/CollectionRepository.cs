using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly LiteCollection<Collection> _collectionCollection;

        public CollectionRepository(IDbContext context)
        {
            _collectionCollection = context.GetContext().GetCollection<Collection>();
        }
        public IEnumerable<Collection> GetCollectionByTypes(IEnumerable<CollectionType> types)
        {
            return _collectionCollection.Find(x => types.Any(y => y == x.Type));
        }

        public void AddOrUpdateRange(IEnumerable<Collection> collections)
        {
            _collectionCollection.Upsert(collections);
        }
    }
}
