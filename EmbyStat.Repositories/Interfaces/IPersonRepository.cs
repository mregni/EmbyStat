using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void UpserRange(IEnumerable<Person> people);
        IEnumerable<string> GetIds();
        Person GetPersonById(string id);
        void AddOrUpdatePerson(Person person);
        void CleanupPersons();
    }
}
