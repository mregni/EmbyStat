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

        public SmallSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService, IEmbyService embyService) : base(hubHelper, jobRepository, configurationService)
        {
            _embyService = embyService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallEmbySyncJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            var server = await _embyService.GetLiveServerInfo();
            LogInformation("Server info found");
            LogProgress(35);

            var pluginsResponse = await _embyService.GetLivePluginInfo();
            LogInformation("Server plugins found");
            LogProgress(55);

            var drives = await _embyService.GetLiveEmbyDriveInfo();
            LogInformation("Server drives found");
            LogProgress(75);

            _embyService.UpdateOrAddServerInfo(server);
            LogProgress(85);

            _embyService.RemoveAllAndInsertPluginRange(pluginsResponse);
            LogProgress(95);

            _embyService.RemoveAllAndInsertDriveRange(drives);
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