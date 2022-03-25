using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository
    {
        #region Shows
        Task UpsertShows(IEnumerable<SqlShow> shows);
        Task<IEnumerable<SqlShow>> GetAllShowsWithEpisodes();
        Task<SqlShow> GetShowByIdWithEpisodes(string showId);
        void RemoveShows();
        Task<Dictionary<SqlShow, int>> GetShowsWithMostEpisodes(int count);
        Task<IEnumerable<SqlShow>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters);
        IEnumerable<SqlShow> GetShowsWithMostDiskSpaceUsed(int i);

        Task<int> CompleteCollectedCount();
        Task DeleteAll();
        #endregion

        #region Charts
        Task<Dictionary<string, int>> GetShowStatusCharValues();
        Task<IEnumerable<double>> GetCollectedRateChart();
        #endregion

        #region Episodes
        IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId);
        Task<int> GetEpisodeCount(LocationType locationType);
        Task<long> GetTotalRunTimeTicks();
        Task<double> GetTotalDiskSpaceUsed();
        #endregion
    }
}
