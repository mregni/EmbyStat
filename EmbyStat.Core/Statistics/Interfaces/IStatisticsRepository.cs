using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Statistics.Interfaces;

public interface IStatisticsRepository
{
    Task<Statistic> GetLastResultByType(StatisticType type);
    Task ReplaceStatistic(string json, DateTime calculationDateTime, StatisticType type);
    Task DeleteStatistics();
    Task MarkTypesAsInvalid(StatisticType type);
}