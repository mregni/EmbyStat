using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void InsertShowsBulk(IEnumerable<Show> list);
        void InsertSeasonsBulk(IEnumerable<Season> seasons);
        void InsertEpisodesBulk(IEnumerable<Episode> episodes);
        IEnumerable<Show> GetAllShowsWithTvdbId();
        IEnumerable<Episode> GetAllEpisodesForShow(int showId);
        int CountEpisodes(int showId);
        int GetEpisodeCountForShow(int showId);
        int GetEpisodeCountForShow(int showId, bool includeSpecials);
        int GetSeasonCountForShow(int showId);
        int GetSeasonCountForShow(int showId, bool includeSpecials);
        bool AnyShows();
        Episode GetEpisodeById(string id);
        IEnumerable<Show> GetAllShows(IReadOnlyList<string> collectionIds);
        IEnumerable<Show> GetAllShows(IReadOnlyList<string> collectionIds, bool includeSeasons, bool includeEpisodes);
        Season GetSeasonById(string id);
        int GetShowCountForPerson(string personId);
    }
}
