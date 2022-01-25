using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void Upsert(Person person);
        Task UpsertRange(IEnumerable<SqlPerson> people);
        Person GetPersonByName(string name);
    }
}
