using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        ShowStat GetGeneralStats(List<string> collectionIds);
        ShowGraphs GetGraphs(List<string> collectionIds);
        Task<PersonStats> GetPeopleStats(List<string> collectionsIds);
    }
}
