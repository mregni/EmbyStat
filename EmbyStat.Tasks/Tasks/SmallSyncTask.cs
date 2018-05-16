using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EmbyStat.Tasks.Tasks
{
    public class SmallSyncTask : IScheduledTask
    {
        private readonly IEmbyClient _embyClient;
        private readonly IPluginRepository _embyPluginRepository;
        private readonly IServerInfoRepository _embyServerInfoRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IDriveRepository _embyDriveRepository;

        public SmallSyncTask(IApplicationBuilder app)
        {
            _embyClient = app.ApplicationServices.GetService<IEmbyClient>();
            _embyPluginRepository = app.ApplicationServices.GetService<IPluginRepository>();
            _embyServerInfoRepository = app.ApplicationServices.GetService<IServerInfoRepository>();
            _configurationRepository = app.ApplicationServices.GetService<IConfigurationRepository>();
            _embyDriveRepository = app.ApplicationServices.GetService<IDriveRepository>();
        }

        public string Name => "Small sync with Emby";
        public string Key => "SmallEmbySync";
        public string Description => "TASKS.SMALLEMBYSYNCDESCRIPTION";
        public string Category => "Emby";
        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger logProgress)
        {
            var settings = _configurationRepository.GetSingle();
            if (!settings.WizardFinished)
            {
                Log.Warning("Movie sync task not running because wizard is not finished yet!");
                return;
            }

            progress.Report(15);

            _embyClient.SetAddressAndUrl(settings.EmbyServerAddress, settings.AccessToken);
            var systemInfoReponse = await _embyClient.GetServerInfoAsync();
            logProgress.LogInformation("Server info found");
            progress.Report(35);
            var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
            logProgress.LogInformation("Server plugins found");
            progress.Report(55);
            var drives = await _embyClient.GetLocalDrivesAsync();
            logProgress.LogInformation("Server drives found");
            progress.Report(75);

            var systemInfo = Mapper.Map<ServerInfo>(systemInfoReponse);
            var localDrives = Mapper.Map<IList<Drives>>(drives);

            _embyServerInfoRepository.UpdateOrAdd(systemInfo);
            progress.Report(85);
            _embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
            progress.Report(95);
            _embyDriveRepository.ClearAndInsertList(localDrives.ToList());

            logProgress.LogInformation("All server data is saved");
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
