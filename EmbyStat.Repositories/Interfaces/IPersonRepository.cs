using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void AddRangeIfMissing(IEnumerable<Person> people);
        List<Guid> GetIds();
        Person GetPersonById(Guid id);
        void AddOrUpdatePerson(Person person);
        Task CleanupPersons();
    }
}
