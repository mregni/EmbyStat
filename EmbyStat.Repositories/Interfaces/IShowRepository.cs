using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void UpdateShow(Show show);
        void AddRange(IEnumerable<Show> list);
        void AddRange(IEnumerable<Season> list);
        void AddRange(IEnumerable<Episode> list);
        IEnumerable<Show> GetAllShows(IEnumerable<Guid> collections, bool inludeSubs = false);
        IEnumerable<Season> GetAllSeasonsForShow(Guid showId, bool inludeSubs = false);
        IEnumerable<Episode> GetAllEpisodesForShow(Guid showId, bool inludeSubs = false);
        IEnumerable<Episode> GetAllEpisodesForShows(IEnumerable<Guid> showIds, bool inludeSubs = false);
        void SetTvdbSynced(Guid showId);
        int CountShows(IEnumerable<Guid> collectionIds);
        int CountEpisodes(Guid showId);
        int CountEpisodes(IEnumerable<Guid> collectionIds);
        long GetPlayLength(IEnumerable<Guid> collectionIds);
        int GetTotalPersonByType(IEnumerable<Guid> collections, string type);
        Guid GetMostFeaturedPerson(IEnumerable<Guid> collections, string type);
        List<Guid> GetGenres(IEnumerable<Guid> collections);
        int GetEpisodeCountForShow(Guid showId, bool includeSpecials = false);
        int GetSeasonCountForShow(Guid showId, bool includeSpecials = false);
        bool Any();
    }
}
