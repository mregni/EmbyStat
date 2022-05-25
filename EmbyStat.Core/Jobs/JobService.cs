using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.Jobs.Interfaces;

namespace EmbyStat.Core.Jobs;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public IEnumerable<Job> GetAll()
    {
        return _jobRepository.GetAll();
    }

    public Job GetById(Guid id)
    {
        return _jobRepository.GetById(id);
    }

    public Task<bool> UpdateTrigger(Guid id, string trigger)
    {
        return _jobRepository.UpdateTrigger(id, trigger);
    }

    public Task ResetAllJobs()
    {
        return _jobRepository.ResetAllJobs();
    }
}