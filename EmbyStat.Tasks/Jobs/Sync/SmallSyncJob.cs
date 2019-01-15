using System;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
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
        private readonly IConfigurationRepository _configurationRepository;

        public SmallSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService, IEmbyService embyService, IConfigurationRepository configurationRepository) : base(hubHelper, jobRepository, configurationService)
        {
            _embyService = embyService;
            _configurationRepository = configurationRepository;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallEmbySyncJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            var settings = _configurationRepository.GetConfiguration();
            LogProgress(15);

            _embyService.SetEmbyClientAddressAndUrl(settings.GetFullEmbyServerAddress(), settings.AccessToken);
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

        public override void Dispose()
        {
            _embyService.Dispose();
        }
    }
}