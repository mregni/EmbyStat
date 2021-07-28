using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{

    public class StatisticsRepository : BaseRepository, IStatisticsRepository
    {
        public StatisticsRepository(IDbContext context) : base(context)
        {
        }

        public Statistic GetLastResultByType(StatisticType type, IReadOnlyList<string> collectionIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Statistic>();
                return collection.Find(x => x.IsValid && x.Type == type)
                    .GetStatisticsWithCollectionIds(collectionIds)
                    .OrderByDescending(x => x.CalculationDateTime)
                    .FirstOrDefault();
            });
        }

        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IReadOnlyList<string> collectionIds)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Statistic>();
                var statistics = collection
                    .Find(x => x.Type == type)
                    .GetStatisticsWithCollectionIds(collectionIds)
                    .ToList();

                statistics.ForEach(x => x.IsValid = false);
                collection.Update(statistics);

                var statistic = new Statistic
                {
                    CalculationDateTime = calculationDateTime,
                    CollectionIds = collectionIds,
                    Type = type,
                    JsonResult = json,
                    IsValid = true
                };

                collection.Insert(statistic);
            });
        }

        public void CleanupStatistics()
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Statistic>();
                collection.DeleteMany(x => !x.IsValid);
            });
        }

        public void MarkMovieTypesAsInvalid()
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Statistic>();
                var statistics = collection.Find(x => x.IsValid && x.Type == StatisticType.Movie).ToList();
                statistics.ForEach(x => x.IsValid = false);
                collection.Update(statistics);
            });
        }

        public void MarkShowTypesAsInvalid()
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Statistic>();
                var statistics = collection.Find(x => x.IsValid && x.Type == StatisticType.Show).ToList();
                statistics.ForEach(x => x.IsValid = false);
                collection.Update(statistics);
            });
        }
    }
}
