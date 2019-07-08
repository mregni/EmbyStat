using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void UpsertShows(IEnumerable<Show> list);
        void UpsertSeasons(IEnumerable<Season> seasons);
        void UpsertEpisodes(IEnumerable<Episode> episodes);
        IEnumerable<Show> GetAllShowsWithTvdbId();
        IEnumerable<Season> GetAllSeasonsForShow(string showId);
        IEnumerable<Episode> GetAllEpisodesForShow(string showId);
        IEnumerable<Episode> GetAllEpisodesForShows(IEnumerable<string> showIds);
        int CountShows(IEnumerable<string> collectionIds);
        int CountEpisodes(string showId);
        int CountEpisodes(IEnumerable<string> collectionIds);
        long GetPlayLength(IEnumerable<string> collectionIds);
        int GetTotalPeopleByType(IEnumerable<string> collectionIds, string type);
        string GetMostFeaturedPerson(IEnumerable<string> collectionIds, string type);
        IEnumerable<string> GetGenres(IEnumerable<string> collectionIds);
        int GetEpisodeCountForShow(string showId, bool includeSpecials = false);
        int GetSeasonCountForShow(string showId, bool includeSpecials = false);
        bool Any();
        Episode GetEpisodeById(string id);
        IEnumerable<Show> GetAllShows(IEnumerable<string> collectionIds);
        Season GetSeasonById(string id);
    }
}
