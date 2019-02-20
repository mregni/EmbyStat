using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Updater
{
    [DisableConcurrentExecution(30)]
    public class CheckUpdateJob : BaseJob, ICheckUpdateJob
    {
        private readonly IUpdateService _updateService;
        private readonly ISettingsService _settingsService;

        public CheckUpdateJob(IJobHubHelper hubHelper, IJobRepository jobRepository,
            ISettingsService settingsService, IUpdateService updateService) 
            : base(hubHelper, jobRepository, settingsService)
        {
            _updateService = updateService;
            _settingsService = settingsService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.CheckUpdateId;
        public override string JobPrefix => Constants.LogPrefix.CheckUpdateJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            await LogInformation("Contacting Github now to see if new version is available.");
            var update = await _updateService.CheckForUpdate(Settings, new CancellationToken(false));
            await LogProgress(20);
            if (update.IsUpdateAvailable && Settings.AutoUpdate)
            {
                await LogInformation($"New version found: v{update.AvailableVersion}");
                await LogInformation($"Auto update is enabled so going to update the server now!");
                await _settingsService.SetUpdateInProgressSetting(true);
                await HubHelper.BroadcastUpdateState(true);
                Task.WaitAll(_updateService.DownloadZip(update));
                await LogProgress(50);
                await _updateService.UpdateServer();
            }
            else if (update.IsUpdateAvailable)
            {
                await LogInformation($"New version found: v{update.AvailableVersion}");
                await LogInformation("Auto updater is disabled, so going to end task now.");
            }
            else
            {
                await LogInformation("No new version available");
            }
        }

        public override async void OnFail()
        {
            await _settingsService.SetUpdateInProgressSetting(false);
            await HubHelper.BroadcastUpdateState(false);
        }

        public override void Dispose()
        {

        }
    }
}