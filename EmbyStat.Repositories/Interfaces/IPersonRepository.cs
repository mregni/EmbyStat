using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        void Upsert(Person person);
        Person GetPersonByName(string name);
    }
}
