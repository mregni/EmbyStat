using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    .OrderByDescending(x => x.CalculationDateTime)
                    .FirstOrDefault();
            }
        }

        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type, IEnumerable<string> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                var collectionList = collections.Select(x => new StatisticCollection
                {
                    Id = Guid.NewGuid().ToString(),
                    StatisticId = x
                }).ToList();

                var result = new Statistic
                {
                    CalculationDateTime = calculationDateTime,
                    Collections = collectionList,
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    JsonResult = json
                };

                context.Statistics.Add(result);
                context.SaveChanges();
            }
        }
    }
}
