using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Models.Charts;

namespace EmbyStat.Services.Abstract
{
    public abstract class MediaService
    {
        private readonly IJobRepository _jobRepository;

        protected MediaService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public bool StatisticsAreValid(Statistic statistic, IEnumerable<string> collectionIds)
        {
            var lastMediaSync = _jobRepository.GetById(Constants.JobIds.MediaSyncId);

            return statistic != null
                   && lastMediaSync != null
                   && statistic.CalculationDateTime > lastMediaSync.EndTimeUtc
                   && collectionIds.AreListEqual(statistic.CollectionIds);
        }

        public Chart CalculateRatingChart(IEnumerable<float?> list)
        {
            var ratingDataList = list
                .GroupBy(x => x.RoundToHalf())
                .OrderBy(x => x.Key)
                .ToList();

            for (double i = 0; i < 10; i += 0.5)
            {
                if (!ratingDataList.Any(x => x.Key == i))
                {
                    ratingDataList.Add(new ChartGrouping<double?, float?> { Key = i, Capacity = 0 });
                }
            }
            
            var ratingData = ratingDataList
                .Select(x => new { Name = x.Key?.ToString() ?? Constants.Unknown, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerCommunityRating,
                Labels = ratingData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { ratingData.Select(x => x.Count) }
            };
        }

        protected Chart CalculatePremiereYearChart(IEnumerable<DateTimeOffset?> list)
        {
            var yearDataList = list
                .GroupBy(x => x.RoundToFiveYear())
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
                        yearDataList.Add(new ChartGrouping<int?, DateTimeOffset?> { Key = i, Capacity = 0 });
                    }
                }
            }

            var yearData = yearDataList
                .Select(x => new { Name = x.Key != null ? $"{x.Key} - {x.Key + 4}" : Constants.Unknown, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerPremiereYear,
                Labels = yearData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { yearData.Select(x => x.Count) }
            };
        }
    }
}
