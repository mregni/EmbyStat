using EmbyStat.Common.Enums.StatisticEnum;

namespace EmbyStat.Core.Statistics.Interfaces;

public interface IMovieStatisticsService
{
    Task<string> CalculateStatistic(StatisticCardType cardType, Statistic uniqueType);
}