using System;
using EmbyStat.Common;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs
{
    public class JobInitializer : IJobInitializer, IDisposable
    {
        private readonly IDatabaseCleanupJob _databaseCleanupJob;
        private readonly IPingEmbyJob _pingEmbyJob;
        private readonly IMediaSyncJob _mediaSyncJob;
        private readonly ISmallSyncJob _smallSyncJob;
        private readonly ICheckUpdateJob _checkUpdateJob;
        private readonly IConfigurationService _configurationService;

        public JobInitializer(IDatabaseCleanupJob databaseCleanupJob, IPingEmbyJob pingEmbyJob, IMediaSyncJob mediaSyncJob, 
            ISmallSyncJob smallSyncJob, ICheckUpdateJob checkUpdateJob, IConfigurationService configurationService)
        {
            _databaseCleanupJob = databaseCleanupJob;
            _pingEmbyJob = pingEmbyJob;
            _mediaSyncJob = mediaSyncJob;
            _smallSyncJob = smallSyncJob;
            _checkUpdateJob = checkUpdateJob;
            _configurationService = configurationService;
        }

        public void Setup()
        {
            var settings = _configurationService.GetServerSettings();

            RecurringJob.AddOrUpdate(Constants.JobIds.DatabaseCleanupId.ToString(),() => _databaseCleanupJob.Execute(), settings.DatabaseCleanupJobTrigger);
            RecurringJob.AddOrUpdate(Constants.JobIds.PingEmbyId.ToString(),() => _pingEmbyJob.Execute(), settings.PingEmbyJobTrigger);
            RecurringJob.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(), () => _mediaSyncJob.Execute(), settings.MediaSyncJobTrigger);
            RecurringJob.AddOrUpdate(Constants.JobIds.SmallSyncId.ToString(), () => _smallSyncJob.Execute(), settings.SmallSyncJobTrigger);
            RecurringJob.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(), () => _checkUpdateJob.Execute(), settings.UpdateCheckJobTrigger);
        }

        public void Dispose()
        {
            _databaseCleanupJob.Dispose();
            _pingEmbyJob.Dispose();
            _mediaSyncJob.Dispose();
            _smallSyncJob.Dispose();
            _checkUpdateJob.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}