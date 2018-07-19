using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Models.Graph;

namespace EmbyStat.Services.Abstract
{
    public abstract class MediaService
    {
        private readonly ITaskRepository _taskRepository;

        protected MediaService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public bool NewStatisticsNeeded(Statistic statistic, IEnumerable<string> collectionIds)
        {
            var lastMediaSync = _taskRepository.GetLatestTaskByKeyAndStatus("MediaSync", TaskCompletionStatus.Completed);

            return statistic != null
                   && lastMediaSync != null
                   && statistic.CalculationDateTime > lastMediaSync.EndTimeUtc
                   && collectionIds.AreListEqual(statistic.Collections.Select(x => x.CId).ToList());
        }

        public Graph<SimpleGraphValue> CalculateRatingGraph(IEnumerable<float?> movies)
        {
            var ratingDataList = movies
                .GroupBy(x => x.RoundToHalf())
                .OrderBy(x => x.Key)
                .ToList();

            var j = 0;
            for (double i = 0; i < 10; i += 0.5)
            {
                if (ratingDataList[j].Key != i)
                {
                    ratingDataList.Add(new GraphGrouping<double?, float?> { Key = i, Capacity = 0 });
                }
                else
                {
                    j++;
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
    }
}
