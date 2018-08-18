using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Statistic GetLastResultByType(StatisticType type);
        void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<Guid> collections);
        Task CleanupStatistics();
        void MarkMovieTypesAsInvalid();
        void MarkShowTypesAsInvalid();
    }
}
