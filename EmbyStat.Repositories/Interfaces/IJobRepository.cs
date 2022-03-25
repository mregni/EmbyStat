using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IJobRepository
    {
        IEnumerable<SqlJob> GetAll();
        SqlJob GetById(Guid id);
        Task StartJob(Guid id);
        Task EndJob(Guid id, DateTime endTime, JobState state);
        Task<bool> UpdateTrigger(Guid id, string trigger);
        Task ResetAllJobs();
    }
}