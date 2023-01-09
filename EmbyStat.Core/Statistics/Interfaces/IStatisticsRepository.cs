using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Statistics;

namespace EmbyStat.Core.Statistics.Interfaces;

public interface IStatisticsRepository
{
    Task<StatisticOld> GetLastResultByType(StatisticType type);
    Task ReplaceStatistic(string json, DateTime calculationDateTime, StatisticType type);
    Task DeleteStatistics();
    Task MarkTypesAsInvalid(StatisticType type);



    Task<List<StatisticCard>> GetCardsByType(StatisticType type);
    Task SaveChangesAsync();
    Task<StatisticPage?> GetPage(Guid id);
}