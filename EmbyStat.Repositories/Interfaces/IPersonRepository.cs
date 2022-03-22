using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        Task UpsertRange(IEnumerable<SqlPerson> people);
        Task DeleteAll();
    }
}
