using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60)]
    public class SmallSyncJob : BaseJob, ISmallSyncJob
    {
        private readonly IEmbyService _embyService;

        public SmallSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, IEmbyService embyService) : base(hubHelper, jobRepository, settingsService)
        {
            _embyService = embyService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallEmbySyncJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            await _embyService.GetAndProcessServerInfo(Settings.Emby.FullEmbyServerAddress, Settings.Emby.AccessToken);
            await LogInformation("Server info downloaded");
            await LogProgress(35);

            await _embyService.GetAndProcessPluginInfo(Settings.Emby.FullEmbyServerAddress, Settings.Emby.AccessToken);
            await LogInformation("Server plugins downloaded");
            await LogProgress(55);

            await _embyService.GetAndProcessEmbyUsers(Settings.Emby.FullEmbyServerAddress, Settings.Emby.AccessToken);
            await LogInformation("Server users downloaded");
            await LogProgress(80);

            await _embyService.GetAndProcessDevices(Settings.Emby.FullEmbyServerAddress, Settings.Emby.AccessToken);
            await LogInformation("Server devices downloaded");
        }

        public void Dispose()
        {
            _embyService.Dispose();
        }
    }
}