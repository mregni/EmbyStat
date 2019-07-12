using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
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

        public Statistic GetLastResultByType(StatisticType type, IReadOnlyList<string> collectionIds)
        {
            return _statisticCollection.Find(Query.And(Query.EQ("Type", type.ToString()), Query.EQ("IsValid", true)))
                .GetStatisticsWithCollectionIds(collectionIds)
                .OrderByDescending(x => x.CalculationDateTime)
                .FirstOrDefault();
        }

        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<string> collectionIds)
        {
            var statisticObjs = _statisticCollection
                .Find(x => x.Type == type)
                .Where(x => x.CollectionIds.All(collectionIds.Contains))
                .Where(x => x.CollectionIds.Count() == collectionIds.Count()).ToList();

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
            var statistics = _statisticCollection.Find(Query.And(Query.EQ("IsValid", true), Query.EQ("Type", StatisticType.Movie.ToString()))).ToList();
            statistics.ForEach(x => x.IsValid = false);
            _statisticCollection.Update(statistics);
        }

        public void MarkShowTypesAsInvalid()
        {
            var statistics = _statisticCollection.Find(Query.And(Query.EQ("IsValid", true), Query.EQ("Type", StatisticType.Show.ToString()))).ToList();
            statistics.ForEach(x => x.IsValid = false);
            _statisticCollection.Update(statistics);
        }
    }
}
