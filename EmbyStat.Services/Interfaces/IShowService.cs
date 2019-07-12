using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        ShowStatistics GetStatistics(List<string> collectionIds);
        List<ShowCollectionRow> GetCollectionRows(List<string> collectionIds);
        bool TypeIsPresent();
    }
}
