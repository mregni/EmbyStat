using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Repositories.Interfaces.Helpers
{
    public interface IMediaRepository
    {
        IEnumerable<SqlMedia> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMedia> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMedia> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMedia> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMedia> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        Task<int> Count(IReadOnlyList<string> libraryIds);
        Task<int> Count(Filter[] filters, IReadOnlyList<string> libraryIds);
        bool Any();
        int GetMediaCountForPerson(string name, string genre);
        int GetMediaCountForPerson(string name);
        Task<int> GetGenreCount(IReadOnlyList<string> libraryIds);
        int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type);
        IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count);
    }
}
