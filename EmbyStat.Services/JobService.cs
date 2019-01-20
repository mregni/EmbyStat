using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
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

        public bool UpdateTrigger(Guid id, string trigger)
        {
            return _jobRepository.UpdateTrigger(id, trigger);
        }

        public void ResetAllJobs()
        {
            _jobRepository.ResetAllJobs();
        }
    }
}
