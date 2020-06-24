using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILibraryRepository
    {
        List<Library> GetLibrariesById(IEnumerable<string> ids);
        void AddOrUpdateRange(IEnumerable<Library> collections);
    }
}
