using EmbyStat.Common.Models.Entities;
using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IJobRepository
    {
        IEnumerable<Job> GetAll();
        Job GetById(Guid id);
        void StartJob(Job job);
        void EndJob(Guid id, DateTime endTime, JobState state);
        bool UpdateTrigger(Guid id, string trigger);
    }
}