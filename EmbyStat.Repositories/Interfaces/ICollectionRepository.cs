using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ICollectionRepository
    {
        IEnumerable<Collection> GetCollectionByTypes(IEnumerable<CollectionType> types);
        void AddCollectionRange(IEnumerable<Collection> collections);
        void AddOrUpdateRange(IEnumerable<Collection> collections);
    }
}
