using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        void AddRangeIfMissing(IEnumerable<Genre> genres);
        List<string> GetIds();
        List<Genre> GetListByIds(List<string> ids);
    }
}
