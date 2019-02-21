using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Statistic GetLastResultByType(StatisticType type);
        Task AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<string> collections);
        Task CleanupStatistics();
        Task MarkMovieTypesAsInvalid();
        Task MarkShowTypesAsInvalid();
    }
}
