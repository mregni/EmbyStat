using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void UpserRange(IEnumerable<Person> people);
        Person GetPersonByName(string id);
    }
}
