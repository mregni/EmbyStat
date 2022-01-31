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
        void UpsertShows(IEnumerable<SqlShow> shows);
        void UpsertShow(SqlShow show);
        void InsertSeasons(IEnumerable<SqlSeason> seasons);
        void InsertEpisodes(IEnumerable<SqlEpisode> episodes);
        IEnumerable<SqlShow> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes);
        SqlShow GetShowById(string showId, bool includeEpisodes);
        Task<SqlSeason> GetSeasonById(string id);
        IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId);
        SqlShow GetShowById(string showId);
        void RemoveShowsThatAreNotUpdated(DateTime startTime);
        void AddEpisode(SqlEpisode episode);
        void RemoveShows();
        Dictionary<SqlShow, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlShow> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds);
    }
}
