using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IJobService
    {
        IEnumerable<Job> GetAll();
        Job GetById(Guid id);
        bool UpdateTrigger(Guid id, string trigger);
        void ResetAllJobs();
    }
}
