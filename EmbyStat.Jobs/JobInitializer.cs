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

        public void Setup()
        {
            var jobs = _jobService.GetAll().ToList();

            RecurringJob.AddOrUpdate(
                Constants.JobIds.DatabaseCleanupId.ToString(),
                () => _databaseCleanupJob.Execute(),
                jobs.Single(x => x.Id == Constants.JobIds.DatabaseCleanupId).Trigger);
            RecurringJob.AddOrUpdate(
                Constants.JobIds.PingEmbyId.ToString(),
                () => _pingEmbyJob.Execute(),
                jobs.Single(x => x.Id == Constants.JobIds.PingEmbyId).Trigger);
            RecurringJob.AddOrUpdate(
                Constants.JobIds.MediaSyncId.ToString(),
                () => _mediaSyncJob.Execute(),
                jobs.Single(x => x.Id == Constants.JobIds.MediaSyncId).Trigger);
            RecurringJob.AddOrUpdate(
                Constants.JobIds.SmallSyncId.ToString(),
                () => _smallSyncJob.Execute(),
                jobs.Single(x => x.Id == Constants.JobIds.SmallSyncId).Trigger);
            RecurringJob.AddOrUpdate(
                Constants.JobIds.CheckUpdateId.ToString(),
                () => _checkUpdateJob.Execute(),
                jobs.Single(x => x.Id == Constants.JobIds.CheckUpdateId).Trigger);
            
        }

        public void UpdateTrigger(Guid id, string trigger)
        {
            if (id == Constants.JobIds.MediaSyncId)
            {
                RecurringJob.AddOrUpdate(id.ToString(), () => _mediaSyncJob.Execute(), trigger);
            }
            else if (id == Constants.JobIds.CheckUpdateId)
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