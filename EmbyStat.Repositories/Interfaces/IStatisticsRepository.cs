using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Statistic GetLastResultByType(StatisticType type);
        void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<Guid> collections);
    }
}
