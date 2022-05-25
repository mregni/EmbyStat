using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Jobs.Interfaces;

public interface IJobService
{
    IEnumerable<Job> GetAll();
    Job GetById(Guid id);
    Task<bool> UpdateTrigger(Guid id, string trigger);
    Task ResetAllJobs();
}