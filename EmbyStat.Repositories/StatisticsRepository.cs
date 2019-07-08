using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{

    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly LiteCollection<Statistic> _statisticCollection;

        public StatisticsRepository(IDbContext context)
        {
            _statisticCollection = context.GetContext().GetCollection<Statistic>();
        }

        public Statistic GetLastResultByType(StatisticType type)
        {
            return _statisticCollection.Find(Query.And(Query.EQ("Type", (int)type), Query.EQ("IsValid", true)))
                .OrderByDescending(x => x.CalculationDateTime)
                .FirstOrDefault();
        }

        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<string> collectionIds)
        {
            var statisticObjs = _statisticCollection.Find(x => x.Type == type).ToList();
            statisticObjs.ForEach(x => x.IsValid = false);
            _statisticCollection.Update(statisticObjs);

            var statistic = new Statistic
            {
                CalculationDateTime = calculationDateTime,
                CollectionIds = collectionIds,
                Type = type,
                JsonResult = json,
                IsValid = true
            };

            _statisticCollection.Insert(statistic);
        }

        public void CleanupStatistics()
        {
           throw new NotImplementedException();
        }

        public void MarkMovieTypesAsInvalid()
        {
            var bArray = new BsonArray
            {
                (int) StatisticType.MovieGeneral,
                (int) StatisticType.MovieCharts,
                (int) StatisticType.MoviePeople,
                (int) StatisticType.MovieSuspicious
            };

            var statistics = _statisticCollection.Find(Query.And(Query.EQ("IsValid", true), Query.In("Type", bArray))).ToList();
            statistics.ForEach(x => x.IsValid = false);
            _statisticCollection.Update(statistics);
        }

        public void MarkShowTypesAsInvalid()
        {
            var types = new List<StatisticType> { StatisticType.ShowCollected, StatisticType.ShowGeneral, StatisticType.ShowCharts, StatisticType.ShowPeople };
            var bArray = new BsonArray
            {
                (int) StatisticType.ShowCollected,
                (int) StatisticType.ShowGeneral,
                (int) StatisticType.ShowCharts,
                (int) StatisticType.ShowPeople
            };

            var statistics = _statisticCollection.Find(Query.And(Query.EQ("IsValid", true), Query.In("Type", bArray))).ToList();
            statistics.ForEach(x => x.IsValid = false);
            _statisticCollection.Update(statistics);
        }
    }
}
