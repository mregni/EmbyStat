using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void AddRangeIfMissing(IEnumerable<Person> people);
        List<string> GetIds();
        Person GetPersonById(string id);
        void AddOrUpdatePerson(Person person);
        Task CleanupPersons();
    }
}
