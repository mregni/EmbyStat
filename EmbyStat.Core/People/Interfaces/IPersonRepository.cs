using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.People.Interfaces;

public interface IPersonRepository
{
    Task UpsertRange(IEnumerable<Person> people);
    Task DeleteAll();
}