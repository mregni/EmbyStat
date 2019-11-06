using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository<Show>
    {
        void InsertShow(Show show);
        void UpdateShow(Show show);
        void RemoveShows();

        List<Show> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes);
        Episode GetEpisodeById(int id);
        Season GetSeasonById(int id);
        List<Show> GetAllShowsWithTvdbId();
        List<Episode> GetAllEpisodesForShow(int showId);
    }
}
