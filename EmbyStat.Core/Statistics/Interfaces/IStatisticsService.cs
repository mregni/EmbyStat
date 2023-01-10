using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Entities.Statistics;

namespace EmbyStat.Core.Statistics.Interfaces;

public interface IStatisticsService
{
    Task CalculateStatisticsByType(StatisticType type);
    Task<StatisticPage?> CalculatePage(Guid id);
    Task<StatisticPage?> GetPage(Guid id);
    Task CalculateCard(StatisticCard card);
}