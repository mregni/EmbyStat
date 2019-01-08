using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EmbyStat.Tasks.Tasks
{
    public class CheckUpdateTask : IScheduledTask
    {
        private readonly IUpdateService _updateService;
        private readonly IConfigurationRepository _configurationRepository;

        public CheckUpdateTask(IApplicationBuilder app)
        {
            _updateService = app.ApplicationServices.GetService<IUpdateService>();
            _configurationRepository = app.ApplicationServices.GetService<IConfigurationRepository>();
        }

        public string Name => $"TASKS.{Key.ToUpper(CultureInfo.InvariantCulture)}";
        public string Key => "CheckUpdate";
        public string Description => $"TASKS.{Key.ToUpper(CultureInfo.InvariantCulture)}DESCRIPTION";
        public string Category => "System";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger progressLogger)
        {
            progress.Report(0);
            progressLogger.LogInformation(Constants.LogPrefix.CheckUpdateTask, "Embystat update check started.");
            var settings = _configurationRepository.GetConfiguration();

            var update = await _updateService.CheckForUpdate(settings, cancellationToken);

            if (update.IsUpdateAvailable && settings.AutoUpdate)
            {
                _updateService.UpdateServer();
            }

            progressLogger.LogInformation(Constants.LogPrefix.CheckUpdateTask, "Embystat update check completed.");
            progress.Report(100);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ TaskKey = Key, TimeOfDayTicks = 10000, Type = "DailyTrigger"}
            };
        }
    }
}
