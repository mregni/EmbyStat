using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void AddRange(IEnumerable<Show> list);
        void AddRange(IEnumerable<Season> list);
        void AddRange(IEnumerable<Episode> list);
        int CountShows(IEnumerable<string> collectionIds);
        int CountEpisodes(IEnumerable<string> collectionIds);
        long GetPlayLength(IEnumerable<string> collectionIds);
    }
}
