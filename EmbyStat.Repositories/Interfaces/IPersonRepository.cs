using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void AddRangeIfMissing(IEnumerable<Person> people);
        List<string> GetIds();
        Person GetPersonById(string id);
        void AddOrUpdatePerson(Person person);
    }
}
