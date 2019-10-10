using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void InsertShow(Show show);
        void InsertSeasonsBulk(IEnumerable<Season> seasons);
        void InsertEpisodesBulk(IEnumerable<Episode> episodes);
        IEnumerable<Show> GetAllShowsWithTvdbId();
        IEnumerable<Episode> GetAllEpisodesForShow(int showId);
        bool AnyShows();
        Episode GetEpisodeById(string id);
        IEnumerable<Show> GetAllShows(IReadOnlyList<string> collectionIds, bool includeSeasons, bool includeEpisodes);
        Season GetSeasonById(string id);
        int GetShowCountForPerson(string personId);
    }
}
