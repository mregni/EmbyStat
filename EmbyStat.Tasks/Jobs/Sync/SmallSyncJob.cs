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
        private readonly IEmbyClient _embyClient;
        private readonly IPluginRepository _embyPluginRepository;
        private readonly IServerInfoRepository _embyServerInfoRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IDriveRepository _embyDriveRepository;

        public SmallSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService, 
            IEmbyClient embyClient, IPluginRepository embyPluginRepository, IServerInfoRepository embyServerInfoRepository, 
            IConfigurationRepository configurationRepository, IDriveRepository embyDriveRepository) : base(hubHelper, jobRepository, configurationService)
        {
            _embyClient = embyClient;
            _embyPluginRepository = embyPluginRepository;
            _embyServerInfoRepository = embyServerInfoRepository;
            _configurationRepository = configurationRepository;
            _embyDriveRepository = embyDriveRepository;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallEmbySyncJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            var settings = _configurationRepository.GetConfiguration();
            LogProgress(15);

            _embyClient.SetAddressAndUrl(settings.GetFullEmbyServerAddress(), settings.AccessToken);
            var systemInfoReponse = await _embyClient.GetServerInfoAsync();
            LogInformation("Server info found");
            LogProgress(35);

            var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
            LogInformation("Server plugins found");
            LogProgress(55);

            var drives = await _embyClient.GetLocalDrivesAsync();
            LogInformation("Server drives found");
            LogProgress(75);

            _embyServerInfoRepository.UpdateOrAdd(systemInfoReponse);
            LogProgress(85);

            _embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
            LogProgress(95);

            _embyDriveRepository.ClearAndInsertList(drives.ToList());
        }

        public override void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}