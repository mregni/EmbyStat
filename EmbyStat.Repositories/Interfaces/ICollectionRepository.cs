using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ICollectionRepository
    {
        IEnumerable<Collection> GetCollectionByTypes(IEnumerable<CollectionType> types);
        void AddOrUpdateRange(IEnumerable<Collection> collections);
    }
}
