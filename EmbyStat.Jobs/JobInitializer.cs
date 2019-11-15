using System;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs
{
    public class JobInitializer : IJobInitializer
    {
        private readonly IDatabaseCleanupJob _databaseCleanupJob;
        private readonly IPingEmbyJob _pingEmbyJob;
        private readonly IMediaSyncJob _mediaSyncJob;
        private readonly ISmallSyncJob _smallSyncJob;
        private readonly ICheckUpdateJob _checkUpdateJob;
        private readonly IJobService _jobService;

        public JobInitializer(IDatabaseCleanupJob databaseCleanupJob, IPingEmbyJob pingEmbyJob, IMediaSyncJob mediaSyncJob,
            ISmallSyncJob smallSyncJob, ICheckUpdateJob checkUpdateJob, IJobService jobService)
        {
            _databaseCleanupJob = databaseCleanupJob;
            _pingEmbyJob = pingEmbyJob;
            _mediaSyncJob = mediaSyncJob;
            _smallSyncJob = smallSyncJob;
            _checkUpdateJob = checkUpdateJob;
            _jobService = jobService;
        }

        public void Setup(bool disableUpdates)
        {
            var jobs = _jobService.GetAll();
            if (disableUpdates)
            {
                jobs = jobs.Where(x => x.Id != Constants.JobIds.CheckUpdateId);
            }

            foreach (var job in jobs)
            {
                RecurringJob.AddOrUpdate(job.Id.ToString(), () =>_databaseCleanupJob.Execute(), job.Trigger);
            }
        }

        public void UpdateTrigger(Guid id, string trigger, bool disableUpdates)
        {
            if (id == Constants.JobIds.MediaSyncId)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _mediaSyncJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.CheckUpdateId && !disableUpdates)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _checkUpdateJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.DatabaseCleanupId)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _databaseCleanupJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.PingEmbyId)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _pingEmbyJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.SmallSyncId)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _smallSyncJob.Execute(), trigger);
            }
        }
    }
}