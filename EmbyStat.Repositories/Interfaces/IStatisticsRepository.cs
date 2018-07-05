using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        void AddStatistic(string json, DateTime calculationDateTime, StatisticType type);
    }
}
