using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Extensions
{
    public  static class StatisticExtension
    {
        public static IEnumerable<Statistic> GetStatisticsWithCollectionIds(this IEnumerable<Statistic> statistics, IReadOnlyList<string> collectionIds)
        {
            return statistics
                .Where(x => x.CollectionIds.All(collectionIds.Contains))
                .Where(x => x.CollectionIds.Count() == collectionIds.Count);

        }
    }
}
