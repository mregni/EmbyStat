using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository
    {
        #region Shows
        Task UpsertShows(IEnumerable<Show> shows);
        Task<IEnumerable<Show>> GetAllShowsWithEpisodes();
        Task<Show> GetShowByIdWithEpisodes(string showId);
        void RemoveShows();
        Task<Dictionary<Show, int>> GetShowsWithMostEpisodes(int count);
        Task<IEnumerable<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters);
        IEnumerable<Show> GetShowsWithMostDiskSpaceUsed(int i);

        Task<int> CompleteCollectedCount();
        Task DeleteAll();
        #endregion

        #region Charts
        Task<Dictionary<string, int>> GetShowStatusCharValues();
        Task<IEnumerable<double>> GetCollectedRateChart();
        #endregion

        #region Episodes
        IEnumerable<Episode> GetAllEpisodesForShow(string showId);
        Task<int> GetEpisodeCount(LocationType locationType);
        Task<long> GetTotalRunTimeTicks();
        Task<double> GetTotalDiskSpaceUsed();
        #endregion

        IEnumerable<LabelValuePair> CalculateGenreFilterValues();
    }
}
