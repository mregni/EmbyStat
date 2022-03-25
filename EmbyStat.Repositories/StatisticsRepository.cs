using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{

    public class StatisticsRepository : BaseRepository, IStatisticsRepository
    {
        public StatisticsRepository(IDbContext context) : base(context)
        {
        }

        public Statistic GetLastResultByType(StatisticType type)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Statistic>();
            return collection.Find(x => x.IsValid && x.Type == type)
                .OrderByDescending(x => x.CalculationDateTime)
                .FirstOrDefault();
        }

        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Statistic>();
            var statistics = collection
                .Find(x => x.Type == type)
                .ToList();

            statistics.ForEach(x => x.IsValid = false);
            collection.Update(statistics);

            var statistic = new Statistic
            {
                CalculationDateTime = calculationDateTime,
                Type = type,
                JsonResult = json,
                IsValid = true
            };

            collection.Insert(statistic);
        }

        public void DeleteStatistics()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Statistic>();
            collection.DeleteMany(x => !x.IsValid);
        }

        public void MarkMovieTypesAsInvalid()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Statistic>();
            var statistics = collection.Find(x => x.IsValid && x.Type == StatisticType.Movie).ToList();
            statistics.ForEach(x => x.IsValid = false);
            collection.Update(statistics);
        }

        public void MarkShowTypesAsInvalid()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Statistic>();
            var statistics = collection.Find(x => x.IsValid && x.Type == StatisticType.Show).ToList();
            statistics.ForEach(x => x.IsValid = false);
            collection.Update(statistics);
        }
    }
}
