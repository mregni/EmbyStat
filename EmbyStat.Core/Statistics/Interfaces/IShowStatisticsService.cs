using EmbyStat.Common.Enums.StatisticEnum;

namespace EmbyStat.Core.Statistics.Interfaces;

public interface IShowStatisticsService
{
    Task<string> CalculateStatistic(StatisticCardType cardType, Statistic uniqueType);
}