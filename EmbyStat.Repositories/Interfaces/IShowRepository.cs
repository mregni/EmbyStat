using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void AddRange(IEnumerable<Show> list);
        void AddRange(IEnumerable<Season> list);
        void AddRange(IEnumerable<Episode> list);
        IEnumerable<Show> GetAllShows(IEnumerable<string> collections, bool inludeSubs = false);
        IEnumerable<Season> GetAllSeasonsForShow(string showId, bool inludeSubs = false);
        IEnumerable<Episode> GetAllEpisodesForShow(string showId, bool inludeSubs = false);
        void SetTvdbSynced(string showId);
        int CountShows(IEnumerable<string> collectionIds);
        int CountEpisodes(string showId);
        int CountEpisodes(IEnumerable<string> collectionIds);
        long GetPlayLength(IEnumerable<string> collectionIds);
        int GetTotalPersonByType(List<string> collections, string type);
        string GetMostFeaturedPerson(List<string> collections, string type);
        List<Show> GetAll(List<string> collections, bool inludeSubs = false);
        List<string> GetGenres(List<string> collections);
    }
}
