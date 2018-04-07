using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Config;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Repositories.EmbyServerInfo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EmbyStat.Tasks.Tasks
{
    public class SmallSyncTask : IScheduledTask
    {
        private readonly IApplicationBuilder _app;
        private readonly IEmbyClient _embyClient;
        private readonly IEmbyPluginRepository _embyPluginRepository;
        private readonly IEmbyServerInfoRepository _embyServerInfoRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IEmbyDriveRepository _embyDriveRepository;

        public SmallSyncTask(IApplicationBuilder app)
        {
            _app = app;
            _embyClient = app.ApplicationServices.GetService<IEmbyClient>();
            _embyPluginRepository = app.ApplicationServices.GetService<IEmbyPluginRepository>();
            _embyServerInfoRepository = app.ApplicationServices.GetService<IEmbyServerInfoRepository>();
            _configurationRepository = app.ApplicationServices.GetService<IConfigurationRepository>();
            _embyDriveRepository = app.ApplicationServices.GetService<IEmbyDriveRepository>();
        }

        public string Name => "Small sync with Emby";
        public string Key => "SmallEmbySync";
        public string Description => "TASKS.SMALLEMBYSYNCDESCRIPTION";
        public string Category => "Emby";
        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var settings = _configurationRepository.GetSingle();
            progress.Report(15);

            _embyClient.SetAddressAndUrl(settings.EmbyServerAddress, settings.AccessToken);
            var systemInfoReponse = await _embyClient.GetServerInfo();
            progress.Report(35);
            var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
            progress.Report(55);
            var drives = await _embyClient.GetLocalDrives();
            progress.Report(75);

            var systemInfo = Mapper.Map<ServerInfo>(systemInfoReponse);
            var localDrives = Mapper.Map<IList<Drives>>(drives);

            _embyServerInfoRepository.UpdateOrAdd(systemInfo);
            progress.Report(85);
            _embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
            progress.Report(95);
            _embyDriveRepository.ClearAndInsertList(localDrives.ToList());
            progress.Report(100);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ Type = "DailyTrigger", TimeOfDayTicks = 18000000000, TaskKey = Key}
            };
        }
    }
}
