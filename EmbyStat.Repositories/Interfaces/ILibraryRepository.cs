using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILibraryRepository
    {
        IEnumerable<Library> GetLibrariesByTypes(IEnumerable<CollectionType> types);
        void AddOrUpdateRange(IEnumerable<Library> collections);
    }
}
