using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite;
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

        public IEnumerable<SqlJob> GetAll()
        {
            return _jobRepository.GetAll();
        }

        public SqlJob GetById(Guid id)
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
}
