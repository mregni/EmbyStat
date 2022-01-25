using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task UpsertRange(IEnumerable<SqlGenre> genres);
        Task<IEnumerable<SqlGenre>> GetAll();
    }
}
