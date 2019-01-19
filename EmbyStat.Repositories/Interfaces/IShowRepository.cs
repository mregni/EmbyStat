using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void AddRange(IEnumerable<Show> list);
        void AddRange(IEnumerable<Season> list);
        void AddRange(IEnumerable<Episode> list);
        IEnumerable<Show> GetAllShows(IEnumerable<string> collections, bool includeSubs = false);
        IEnumerable<Season> GetAllSeasonsForShow(string showId, bool includeSubs = false);
        IEnumerable<Episode> GetAllEpisodesForShow(string showId, bool includeSubs = false);
        IEnumerable<Episode> GetAllEpisodesForShows(IEnumerable<string> showIds, bool includeSubs = false);
        void SetTvdbSynced(string showId);
        int CountShows(IEnumerable<string> collectionIds);
        int CountEpisodes(string showId);
        int CountEpisodes(IEnumerable<string> collectionIds);
        long GetPlayLength(IEnumerable<string> collectionIds);
        int GetTotalPersonByType(IEnumerable<string> collections, PersonType type);
        string GetMostFeaturedPerson(IEnumerable<string> collections, PersonType type);
        List<string> GetGenres(IEnumerable<string> collections);
        int GetEpisodeCountForShow(string showId, bool includeSpecials = false);
        int GetSeasonCountForShow(string showId, bool includeSpecials = false);
        bool Any();
    }
}
