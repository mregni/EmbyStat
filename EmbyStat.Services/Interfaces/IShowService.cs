using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        ShowStat GetGeneralStats(IEnumerable<string> collectionIds);
        ShowGraphs GetGraphs(IEnumerable<string> collectionIds);
        PersonStats GetPeopleStats(IEnumerable<string> collectionsIds);
        List<ShowCollectionRow> GetCollectionRows(IEnumerable<string> collectionIds);
        bool ShowTypeIsPresent();
    }
}
