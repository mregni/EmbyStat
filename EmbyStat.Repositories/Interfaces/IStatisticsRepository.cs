using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Statistic GetLastResultByType(StatisticType type, IReadOnlyList<string> collectionIds);
        void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<string> collectionIds);
        void CleanupStatistics();
        void MarkMovieTypesAsInvalid();
        void MarkShowTypesAsInvalid();
    }
}
