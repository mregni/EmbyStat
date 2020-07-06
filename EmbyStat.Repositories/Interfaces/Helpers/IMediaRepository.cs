using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Repositories.Interfaces.Helpers
{
    public interface IMediaRepository<out T> where T : Media
    {
        IEnumerable<T> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<T> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<T> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<T> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<T> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        int GetMediaCount(IReadOnlyList<string> libraryIds);
        int GetMediaCount(Filter[] filters, IReadOnlyList<string> libraryIds);
        bool Any();
        int GetMediaCountForPerson(string personId);
    }
}
