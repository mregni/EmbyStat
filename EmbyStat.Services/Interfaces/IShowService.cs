using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        Task<ShowStat> GetGeneralStats(List<string> collectionIds);
        Task<ShowCharts> GetCharts(List<string> collectionIds);
        Task<PersonStats> GetPeopleStats(List<string> collectionsIds);
        Task<List<ShowCollectionRow>> GetCollectionRows(List<string> collectionIds);
        bool TypeIsPresent();
    }
}
