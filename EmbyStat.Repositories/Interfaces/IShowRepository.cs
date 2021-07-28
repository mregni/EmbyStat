using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository<Show>
    {
        void InsertShow(Show show);
        List<Show> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes);
        Show GetShowById(string showId, bool includeEpisodes);
        Season GetSeasonById(string id);
        List<Episode> GetAllEpisodesForShow(string showId);
        Show GetShowById(string showId);
        void RemoveShowsThatAreNotUpdated(DateTime startTime);
        void AddEpisode(Episode episode);
        void RemoveShows();
        Dictionary<Show, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<Show> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds);
    }
}
