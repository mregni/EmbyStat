using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task UpsertRange(IEnumerable<SqlGenre> genres);
        Task<SqlGenre[]> GetAll();
        Task DeleteAll();
    }
}
