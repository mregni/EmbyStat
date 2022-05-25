using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Core.Jobs.Interfaces;

public interface IJobRepository
{
    IEnumerable<Job> GetAll();
    Job GetById(Guid id);
    Task StartJob(Guid id);
    Task EndJob(Guid id, DateTime endTime, JobState state);
    Task<bool> UpdateTrigger(Guid id, string trigger);
    Task ResetAllJobs();
}