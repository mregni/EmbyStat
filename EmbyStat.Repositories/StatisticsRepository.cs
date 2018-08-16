using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        public Statistic GetLastResultByType(StatisticType type)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Statistics
                    .Include(x => x.Collections)
                    .Where(x => x.Type == type)
                    .Where(x => x.IsValid)
                    .OrderByDescending(x => x.CalculationDateTime)
                    .FirstOrDefault();
            }
        }

        public async void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<Guid> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                await context.Statistics.Where(x => x.Type == type).ForEachAsync(x => x.IsValid = false);
                var collectionList = collections.Select(x => new StatisticCollection
                {
                    Id = Guid.NewGuid(),
                    CollectionId = x
                }).ToList();

                var result = new Statistic
                {
                    CalculationDateTime = calculationDateTime,
                    Collections = collectionList,
                    Id = Guid.NewGuid(),
                    Type = type,
                    JsonResult = json,
                    IsValid = true
                };

                context.Statistics.Add(result);
                context.SaveChanges();
            }
        }

        public async Task CleanupStatistics()
        {
            using (var context = new ApplicationDbContext())
            {
                var listToRemove = context.Statistics.Where(x => !x.IsValid);
                context.RemoveRange(listToRemove);
                await context.SaveChangesAsync();
            }
        }

        public async void MarkMovieTypesAsInvalid()
        {
            using (var context = new ApplicationDbContext())
            {
                var types = new List<StatisticType> { StatisticType.MovieGeneral, StatisticType.MovieGraphs, StatisticType.MoviePeople, StatisticType.MovieSuspicious};
                await context.Statistics
                    .Where(x => x.IsValid)
                    .Where(x => types.Any(y => x.Type == y))
                    .ForEachAsync(x => x.IsValid = false);
                await context.SaveChangesAsync();
            }
        }

        public async void MarkShowTypesAsInvalid()
        {
            using (var context = new ApplicationDbContext())
            {
                var types = new List<StatisticType> { StatisticType.ShowCollected, StatisticType.ShowGeneral, StatisticType.ShowGraphs, StatisticType.ShowPeople };
                await context.Statistics
                    .Where(x => x.IsValid)
                    .Where(x => types.Any(y => x.Type == y))
                    .ForEachAsync(x => x.IsValid = false);
                await context.SaveChangesAsync();
            }
        }
    }
}
