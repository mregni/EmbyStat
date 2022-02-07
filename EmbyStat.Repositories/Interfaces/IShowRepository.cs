using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository
    {
        #region Shows

        Task UpsertShows(IEnumerable<SqlShow> shows);
        Task<IEnumerable<SqlShow>> GetAllShowsWithEpisodes(IReadOnlyList<string> libraryIds);
        Task<SqlShow> GetShowByIdWithEpisodes(string showId);
        void RemoveShows();
        Dictionary<SqlShow, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlShow> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds);

        #endregion

        #region Episodes

        IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId);
        Task<int> GetEpisodeCount(IReadOnlyList<string> libraryIds, LocationType locationType);
        Task<long> GetTotalRunTimeTicks(IReadOnlyList<string> libraryIds);
        Task<double> GetTotalDiskSpaceUsed(IReadOnlyList<string> libraryIds);

        #endregion
    }
}
