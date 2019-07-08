using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        void UpsertRange(IEnumerable<Genre> genres);
        IEnumerable<Genre> GetAll();
        IEnumerable<string> GetIds();
        IEnumerable<Genre> GetGenres(IEnumerable<string> ids);
        void CleanupGenres();
    }
}
