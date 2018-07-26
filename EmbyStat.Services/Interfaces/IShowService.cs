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
        ShowStat GetGeneralStats(IEnumerable<Guid> collectionIds);
        ShowGraphs GetGraphs(IEnumerable<Guid> collectionIds);
        PersonStats GetPeopleStats(IEnumerable<Guid> collectionsIds);
        List<ShowCollectionRow> GetCollectionRows(IEnumerable<Guid> collectionIds);
        bool ShowTypeIsPresent();
    }
}
