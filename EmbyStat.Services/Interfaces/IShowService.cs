using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Services.Models.Show;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Collection> GetShowCollections();
        ShowStat GetGeneralStats(List<string> collectionIds);
    }
}
