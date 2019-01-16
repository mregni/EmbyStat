using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Models.Graph;

namespace EmbyStat.Services.Abstract
{
    public abstract class MediaService
    {
        private readonly IJobRepository _jobRepository;

        protected MediaService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public bool StatisticsAreValid(Statistic statistic, IEnumerable<Guid> collectionIds)
        {
            var lastMediaSync = _jobRepository.GetById(Constants.JobIds.MediaSyncId);

            return statistic != null
                   && lastMediaSync != null
                   && statistic.CalculationDateTime > lastMediaSync.EndTimeUtc
                   && collectionIds.AreListEqual(statistic.Collections.Select(x => x.StatisticId).ToList());
        }

        public Graph<SimpleGraphValue> CalculateRatingGraph(IEnumerable<float?> list)
        {
            var ratingDataList = list
                .GroupBy(x => x.RoundToHalf())
                .OrderBy(x => x.Key)
                .ToList();

            if (ratingDataList.Any())
            {
                var j = 0;
                for (double i = 0; i < 10; i += 0.5)
                {
                    var key = ratingDataList[j].Key;
                    if (key != null && key != i)
                    {
                        ratingDataList.Add(new GraphGrouping<double?, float?> {Key = i, Capacity = 0});
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            var ratingData = ratingDataList
                .Select(x => new { Name = x.Key?.ToString() ?? Constants.Unknown, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerCommunityRating,
                Data = ratingData
            };
        }

        protected Graph<SimpleGraphValue> CalculatePremiereYearGraph(IEnumerable<DateTime?> list)
        {
            var yearDataList = list
                .GroupBy(x => x.RoundToFive())
                .OrderBy(x => x.Key)
                .ToList();

            if (yearDataList.Any())
            {
                var lowestYear = yearDataList.Where(x => x.Key.HasValue).Min(x => x.Key);
                var highestYear = yearDataList.Where(x => x.Key.HasValue).Max(x => x.Key);

                var j = 0;
                for (var i = lowestYear.Value; i < highestYear; i += 5)
                {
                    if (yearDataList[j].Key != i)
                    {
                        yearDataList.Add(new GraphGrouping<int?, DateTime?> {Key = i, Capacity = 0});
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            var yearData = yearDataList
                .Select(x => new { Name = x.Key != null ? $"{x.Key} - {x.Key + 4}" : Constants.Unknown, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerPremiereYear,
                Data = yearData
            };
        }
    }
}
