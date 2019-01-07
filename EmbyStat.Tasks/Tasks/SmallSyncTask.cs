using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Interface;
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

        public string Name => "TASKS.SMALLEMBYSYNCTITLE";
        public string Key => "SmallEmbySync";
        public string Description => "TASKS.SMALLEMBYSYNCDESCRIPTION";
        public string Category => "Emby";
        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger logProgress)
        {
            progress.Report(0);
            var settings = _configurationRepository.GetConfiguration();
            if (!settings.WizardFinished)
            {
                logProgress.LogWarning(Constants.LogPrefix.SmallEmbySyncTask, "Media sync task not running because wizard is not finished yet!");
                return;
            }

            logProgress.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Starting a small syncronisation with Emby");
            progress.Report(15);

            _embyClient.SetAddressAndUrl(settings.GetFullEmbyServerAddress(), settings.AccessToken);
            var systemInfoReponse = await _embyClient.GetServerInfoAsync();
            logProgress.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server info found");
            progress.Report(35);
            var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
            logProgress.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server plugins found");
            progress.Report(55);
            var drives = await _embyClient.GetLocalDrivesAsync();
            logProgress.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "Server drives found");
            progress.Report(75);
            _embyServerInfoRepository.UpdateOrAdd(systemInfoReponse);
            progress.Report(85);
            _embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
            progress.Report(95);
            _embyDriveRepository.ClearAndInsertList(drives.ToList());

            logProgress.LogInformation(Constants.LogPrefix.SmallEmbySyncTask, "All server data is saved");
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
