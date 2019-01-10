using System;
using EmbyStat.Services.Interfaces;
using EmbyStat.Tasks.Tasks.Interfaces;
using EmbyStat.Tasks.Tasks.Maintenance;


namespace EmbyStat.Tasks
{
    public class TaskInitializer : ITaskInitializer, IDisposable
    {
        private readonly IDatabaseCleanupTask _databaseCleanupTask;
        private readonly IPingEmbyTask _pingEmbyTask;
        private readonly IMediaSyncTask _mediaSyncTask;
        private readonly ISmallSyncTask _smallSyncTask;
        private readonly ICheckUpdateTask _checkUpdateTask;
        private readonly IConfigurationService _configurationService;

        public TaskInitializer(IDatabaseCleanupTask databaseCleanupTask, IPingEmbyTask pingEmbyTask, IMediaSyncTask mediaSyncTask,
            ISmallSyncTask smallSyncTask, ICheckUpdateTask checkUpdateTask, IConfigurationService configurationService)
        {
            _databaseCleanupTask = databaseCleanupTask;
            _pingEmbyTask = pingEmbyTask;
            _mediaSyncTask = mediaSyncTask;
            _smallSyncTask = smallSyncTask;
            _checkUpdateTask = checkUpdateTask;
            _configurationService = configurationService;
        }

        public void Setup()
        {
            var settings = _configurationService.GetServerSettings();

            //RecurringJob.AddOrUpdate(() => _databaseCleanupTask.Execute(), settings.DatabaseCleanupTaskTrigger);
            //RecurringJob.AddOrUpdate(() => _pingEmbyTask.Execute(), settings.PingEmbyTaskTrigger);
            //RecurringJob.AddOrUpdate(() => _mediaSyncTask.Execute(), settings.MediaSyncTaskTrigger);
            //RecurringJob.AddOrUpdate(() => _smallSyncTask.Execute(), settings.SmallSyncTaskTrigger);
            //RecurringJob.AddOrUpdate(() => _checkUpdateTask.Execute(), settings.UpdateCheckTaskTrigger);
        }

        public void Dispose()
        {
            _databaseCleanupTask.Dispose();
            _pingEmbyTask.Dispose();
            _mediaSyncTask.Dispose();
            _smallSyncTask.Dispose();
            _checkUpdateTask.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}