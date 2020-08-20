using System.Collections.Generic;
using EmbyStat.Common.Enums;
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
        int GetMediaCountForPerson(string name, string genre);
        int GetMediaCountForPerson(string name);
        int GetGenreCount(IReadOnlyList<string> libraryIds);
        int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type);
        IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count);
    }
}
