using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        public void AddStatistic(string json, DateTime calculationDateTime, StatisticType type)
        {
            using (var context = new ApplicationDbContext())
            {
                var result = new Statistic
                {
                    CalculationDateTime = calculationDateTime,
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
