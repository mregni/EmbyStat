using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ICollectionRepository
    {
        IEnumerable<Collection> GetCollectionByType(CollectionType type);
        void AddCollectionRange(IEnumerable<Collection> collections);
        void AddOrUpdateRange(IEnumerable<Collection> collections);
    }
}
