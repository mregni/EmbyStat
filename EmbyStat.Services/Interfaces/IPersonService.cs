using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IPersonService
    {
        Task<Person> GetPersonByNameAsync(string name);
    }
}
