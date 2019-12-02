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
        private readonly IRecurringJobManager _recurringJobManager;

        public JobInitializer(IDatabaseCleanupJob databaseCleanupJob, IPingEmbyJob pingEmbyJob, IMediaSyncJob mediaSyncJob,
            ISmallSyncJob smallSyncJob, ICheckUpdateJob checkUpdateJob, IJobService jobService, IRecurringJobManager recurringJobManager)
        {
            _databaseCleanupJob = databaseCleanupJob;
            _pingEmbyJob = pingEmbyJob;
            _mediaSyncJob = mediaSyncJob;
            _smallSyncJob = smallSyncJob;
            _checkUpdateJob = checkUpdateJob;
            _jobService = jobService;
            _recurringJobManager = recurringJobManager;
        }

        public void Setup(bool disableUpdates)
        {
            var jobs = _jobService.GetAll();
            foreach (var job in jobs)
            {
                UpdateTrigger(job.Id, job.Trigger, disableUpdates);
            }
        }

        public void UpdateTrigger(Guid id, string trigger, bool disableUpdates)
        {
            if (id == Constants.JobIds.MediaSyncId)
            {
                _recurringJobManager.AddOrUpdate(id.ToString(), () => _mediaSyncJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.CheckUpdateId && !disableUpdates)
            {
                _recurringJobManager.AddOrUpdate(id.ToString(), () => _checkUpdateJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.DatabaseCleanupId)
            {
                _recurringJobManager.AddOrUpdate(id.ToString(), () => _databaseCleanupJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.PingEmbyId)
            {
                _recurringJobManager.AddOrUpdate(id.ToString(), () => _pingEmbyJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.SmallSyncId)
            {
                _recurringJobManager.AddOrUpdate(id.ToString(), () => _smallSyncJob.Execute(), trigger);
            }
        }
    }
}