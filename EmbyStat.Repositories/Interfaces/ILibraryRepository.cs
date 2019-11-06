using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILibraryRepository
    {
        List<Library> GetLibrariesByTypes(IEnumerable<LibraryType> types);
        void AddOrUpdateRange(IEnumerable<Library> collections);
    }
}
