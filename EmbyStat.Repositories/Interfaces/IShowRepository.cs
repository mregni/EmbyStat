using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository
    {
        Task UpsertShows(IEnumerable<SqlShow> shows);
        IEnumerable<SqlShow> GetAllShows(IReadOnlyList<string> libraryIds);
        Task<IEnumerable<SqlShow>> GetAllShowsWithEpisodes(IReadOnlyList<string> libraryIds);
        Task<SqlShow> GetShowByIdWithEpisodes(string showId);
        IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId);
        SqlShow GetShowById(string showId);
        void AddEpisode(SqlEpisode episode);
        void RemoveShows();
        Dictionary<SqlShow, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlShow> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds);
    }
}
