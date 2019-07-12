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
            var bArray = new BsonArray();
            foreach (var type in types)
            {
                bArray.Add(type.ToString()); 
            }
            return _collectionCollection.Find(Query.In("Type", bArray));
        }

        public void AddOrUpdateRange(IEnumerable<Collection> collections)
        {
            _collectionCollection.Upsert(collections);
        }
    }
}
