using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Logging;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Charts;
using Newtonsoft.Json;

namespace EmbyStat.Services.Abstract
{
    public abstract class MediaService
    {
        private readonly IJobRepository _jobRepository;
        internal readonly Logger Logger;

        protected MediaService(IJobRepository jobRepository, Type type, string logPrefix)
        {
            _jobRepository = jobRepository;
            Logger = LogFactory.CreateLoggerForType(type, logPrefix);
        }

        internal bool StatisticsAreValid(Statistic statistic, IEnumerable<string> collectionIds, Guid jobId)
        {
            var lastMediaSync = _jobRepository.GetById(jobId);

            //We need to add 5 minutes here because the CalculationDateTime is ALWAYS just in front of the EndTimeUtc :( Ugly fix
            return statistic != null
                   && lastMediaSync != null
                   && statistic.CalculationDateTime.AddMinutes(5) > lastMediaSync.EndTimeUtc
                   && collectionIds.AreListEqual(statistic.CollectionIds);
        }

        internal T CalculateStat<T>(Func<T> action, string errorMessage)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                Logger.Error(e, errorMessage);
            }

            return default;
        }

        #region Chart

        internal static Chart CreateGenreChart(Dictionary<string, int> items)
        {
            var genresData = items
                .Select(x => new SimpleChartData{Label = x.Key, Value = x.Value})
                .ToArray();

            return new Chart
            {
                Title = Constants.CountPerGenre,
                DataSets = genresData,
                SeriesCount = 1
            };
        }

        internal Chart CreateRatingChart(IEnumerable<decimal?> items)
        {
            var ratingDataList = items.GroupBy(x => x.RoundToHalf())
                .OrderBy(x => x.Key)
                .ToList();

            for (double i = 0; i < 10; i += 0.5)
            {
                if (!ratingDataList.Any(x => x.Key == i))
                {
                    ratingDataList.Add(new ChartGrouping<double?, decimal?> { Key = i, Capacity = 0 });
                }
            }

            var ratingData = ratingDataList
                .Select(x => new SimpleChartData { Label = x.Key?.ToString() ?? Constants.Unknown, Value = x.Count() })
                .OrderBy(x => x.Label)
                .ToArray();

            return new Chart
            {
                Title = Constants.CountPerCommunityRating,
                DataSets = ratingData,
                SeriesCount = 1
            };
        }

        internal Chart CalculatePremiereYearChart(IEnumerable<DateTime?> items)
        {
            var yearDataList = items.GroupBy(x => x.RoundToFiveYear())
                .Where(x => x.Key != null)
                .OrderBy(x => x.Key)
                .ToList();

            if (yearDataList.Any())
            {
                var lowestYear = yearDataList.Where(x => x.Key.HasValue).Min(x => x.Key);
                var highestYear = yearDataList.Where(x => x.Key.HasValue).Max(x => x.Key);

                for (var i = lowestYear; i < highestYear; i += 5)
                {
                    if (yearDataList.All(x => x.Key != i))
                    {
                        yearDataList.Add(new ChartGrouping<int?, DateTime?> { Key = i, Capacity = 0 });
                    }
                }
            }

            var yearData = yearDataList
                .Select(x => new SimpleChartData { Label = x.Key != null ? $"{x.Key} - {x.Key + 4}" : Constants.Unknown, Value = x.Count() })
                .OrderBy(x => x.Label)
                .ToArray();

            return new Chart
            {
                Title = Constants.CountPerPremiereYear,
                DataSets = yearData,
                SeriesCount = 1
            };
        }

        internal Chart CalculateOfficialRatingChart(Dictionary<string, int> items)
        {
            var ratingData = items
                .Select(x => new SimpleChartData { Label = x.Key, Value = x.Value })
                .ToArray();

            return new Chart
            {
                Title = Constants.CountPerOfficialRating,
                DataSets = ratingData,
                SeriesCount = 1
            };
        }

        #endregion
    }
}
