using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Repositories.Interfaces.Helpers
{
    public interface IMediaRepository
    {
        IEnumerable<SqlMedia> GetLatestAddedMedia(int count);
        Task<IEnumerable<SqlMedia>> GetNewestPremieredMedia(int count);
        Task<IEnumerable<SqlMedia>> GetOldestPremieredMedia(int count);
        Task<IEnumerable<SqlExtra>> GetHighestRatedMedia(int count);
        Task<IEnumerable<SqlExtra>> GetLowestRatedMedia(int count);

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