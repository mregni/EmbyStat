using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Repositories.Interfaces.Helpers
{
    public interface IMediaRepository<out T> where T : Media
    {
        T GetNewestPremieredMedia(IReadOnlyList<string> libraryIds);
        T GetLatestAddedMedia(IReadOnlyList<string> libraryIds);
        T GetOldestPremieredMedia(IReadOnlyList<string> libraryIds);
        T GetHighestRatedMedia(IReadOnlyList<string> libraryIds);
        T GetLowestRatedMedia(IReadOnlyList<string> libraryIds);
        int GetMediaCount(IReadOnlyList<string> libraryIds);
        bool Any();
        int GetMediaCountForPerson(string personId);
    }
}
