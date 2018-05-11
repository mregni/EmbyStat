using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository
    {
        void RemoveShows();
        void AddRange(List<Show> list);
        void AddRange(List<Season> list);
        void AddRange(List<Episode> list);
    }
}
