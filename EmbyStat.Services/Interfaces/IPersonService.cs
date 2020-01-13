using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IPersonService
    {
        Person GetPersonByName(string name);
    }
}
