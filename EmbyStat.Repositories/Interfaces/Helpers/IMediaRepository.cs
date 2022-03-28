using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Repositories.Interfaces.Helpers
{
    public interface IMediaRepository
    {
        IEnumerable<Media> GetLatestAddedMedia(int count);
        Task<IEnumerable<Media>> GetNewestPremieredMedia(int count);
        Task<IEnumerable<Media>> GetOldestPremieredMedia(int count);
        Task<IEnumerable<Extra>> GetHighestRatedMedia(int count);
        Task<IEnumerable<Extra>> GetLowestRatedMedia(int count);

        #region Charts
        Task<Dictionary<string, int>> GetGenreChartValues();
        IEnumerable<decimal?> GetCommunityRatings();
        IEnumerable<DateTime?> GetPremiereYears();
        Task<Dictionary<string, int>> GetOfficialRatingChartValues();
        #endregion

        Task<int> Count();
        Task<int> Count(Filter[] filters);
        bool Any();
        int GetMediaCountForPerson(string name);
        Task<int> GetGenreCount();
        int GetPeopleCount(PersonType type);
    }
}