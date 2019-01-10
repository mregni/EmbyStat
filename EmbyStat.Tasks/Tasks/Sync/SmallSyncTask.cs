using System;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common.Hubs;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Tasks.Tasks.Interfaces;

namespace EmbyStat.Tasks.Tasks.Sync
{
    public class SmallSyncTask : TaskConnector, ISmallSyncTask
    {
        private readonly IEmbyClient _embyClient;
        private readonly IPluginRepository _embyPluginRepository;
        private readonly IServerInfoRepository _embyServerInfoRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IDriveRepository _embyDriveRepository;

        public SmallSyncTask(IHubHelper hubHelper, IEmbyClient embyClient, IPluginRepository embyPluginRepository,
            IServerInfoRepository embyServerInfoRepository, IConfigurationRepository configurationRepository,
            IDriveRepository embyDriveRepository) : base(hubHelper)
        {
            _embyClient = embyClient;
            _embyPluginRepository = embyPluginRepository;
            _embyServerInfoRepository = embyServerInfoRepository;
            _configurationRepository = configurationRepository;
            _embyDriveRepository = embyDriveRepository;
        }

        public override string Name => "TASKS.SMALLEMBYSYNCTITLE";
        public override string Key => "SmallEmbySync";
        public override string Description => "TASKS.SMALLEMBYSYNCDESCRIPTION";
        public override string Category => "Emby";
        public override Guid Id => new Guid("41e0bf22-1e6b-4f5d-90be-ec966f746a2f");

        public async Task Execute()
        {
            //_progress.Report(0);
            var settings = _configurationRepository.GetConfiguration();
            if (!settings.WizardFinished)
            {
                //_progressLogger.LogWarning(Constants.LogPrefix.SmallEmbySyncTask, "Media sync task not running because wizard is not finished yet!");
                return;
            }

            //_progressLogger.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Starting a small syncronisation with Emby");
            //_progress.Report(15);

            _embyClient.SetAddressAndUrl(settings.GetFullEmbyServerAddress(), settings.AccessToken);
            var systemInfoReponse = await _embyClient.GetServerInfoAsync();
            //_progressLogger.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server info found");
            //_progress.Report(35);
            var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
            //_progressLogger.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server plugins found");
            //_progress.Report(55);
            var drives = await _embyClient.GetLocalDrivesAsync();
            //_progressLogger.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server drives found");
            //_progress.Report(75);
            _embyServerInfoRepository.UpdateOrAdd(systemInfoReponse);
            //_progress.Report(85);
            _embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
            //_progress.Report(95);
            _embyDriveRepository.ClearAndInsertList(drives.ToList());

            //_progressLogger.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "All server data is saved");
            //_progress.Report(100);
        }
        public void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}