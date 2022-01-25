using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IPersonService
    {
        Person GetPersonByNameForMovies(string name);
        Person GetPersonByNameForMovies(string name, string genre);
        Person GetPersonByNameForShows(string name);
        Person GetPersonByNameForShows(string name, string genre);
    }
}
