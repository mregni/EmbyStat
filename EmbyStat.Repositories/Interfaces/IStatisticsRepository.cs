using System;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<Statistic> GetLastResultByType(StatisticType type);
        Task ReplaceStatistic(string json, DateTime calculationDateTime, StatisticType type);
        Task DeleteStatistics();
        Task MarkTypesAsInvalid(StatisticType type);
    }
}
