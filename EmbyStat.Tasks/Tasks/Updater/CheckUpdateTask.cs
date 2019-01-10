using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Hubs;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Tasks.Tasks.Interfaces;

namespace EmbyStat.Tasks.Tasks.Updater
{
    public class CheckUpdateTask : TaskConnector, ICheckUpdateTask
    {
        private readonly IUpdateService _updateService;
        private readonly IConfigurationRepository _configurationRepository;

        public CheckUpdateTask(IHubHelper hubHelper, IUpdateService updateService, IConfigurationRepository configurationRepository) : base(hubHelper)
        {
            _updateService = updateService;
            _configurationRepository = configurationRepository;
        }

        public override string Name => $"TASKS.{Key.ToUpper(CultureInfo.InvariantCulture)}";
        public override string Key => "CheckUpdate";
        public override string Description => $"TASKS.{Key.ToUpper(CultureInfo.InvariantCulture)}DESCRIPTION";
        public override string Category => "System";
        public override Guid Id => new Guid("78bc2bf0-abd9-48ef-aeff-9c396d644f2a");

        public async Task Execute()
        {
            //_progress.Report(0);
            //_progressLogger.LogInformation(Constants.LogPrefix.CheckUpdateTask, "Embystat update check started.");
            var settings = _configurationRepository.GetConfiguration();

            var update = await _updateService.CheckForUpdate(settings, new CancellationToken(false));

            if (update.IsUpdateAvailable && settings.AutoUpdate)
            {
                await _updateService.UpdateServer();
            }

            //_progressLogger.LogInformation(Constants.LogPrefix.CheckUpdateTask, "Embystat update check completed.");
            //_progress.Report(100);
        }

        public void Dispose()
        {
        }
    }
}