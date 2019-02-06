using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        ShowStat GetGeneralStats(List<string> collectionIds);
        ShowGraphs GetGraphs(List<string> collectionIds);
        PersonStats GetPeopleStats(List<string> collectionsIds);
        List<ShowCollectionRow> GetCollectionRows(List<string> collectionIds);
        bool ShowTypeIsPresent();
    }
}
