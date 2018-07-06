using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;

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
    }
}
