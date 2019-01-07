using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        void AddRangeIfMissing(IEnumerable<Genre> genres);
        List<Genre> GetAll();
        List<Guid> GetIds();
        List<Genre> GetListByIds(List<Guid> ids);
        Task CleanupGenres();
    }
}
