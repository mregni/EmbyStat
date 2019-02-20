using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
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

        public override async Task RunJob()
        {
            await _embyService.GetAndProcessServerInfo(Settings.FullEmbyServerAddress, Settings.Emby.AccessToken);
            LogInformation("Server info downloaded");
            LogProgress(35);

            await _embyService.GetAndProcessPluginInfo(Settings.FullEmbyServerAddress, Settings.Emby.AccessToken);
            LogInformation("Server plugins downloaded");
            LogProgress(55);

            await _embyService.GetAndProcessEmbyUsers(Settings.FullEmbyServerAddress, Settings.Emby.AccessToken);
            LogInformation("Server users downloaded");
            LogProgress(80);

            await _embyService.GetAndProcessDevices(Settings.FullEmbyServerAddress, Settings.Emby.AccessToken);
            LogInformation("Server devices downloaded");
        }

        public override void OnFail()
        {
            
        }

        public override void Dispose()
        {
            _embyService.Dispose();
        }
    }
}