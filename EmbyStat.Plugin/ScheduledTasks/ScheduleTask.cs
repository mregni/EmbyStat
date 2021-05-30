using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Plugin.Configuration;
using EmbyStat.Plugin.ScheduledTasks.Calculators;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace EmbyStat.Plugin.ScheduledTasks
{
    public class ScheduleTask : IScheduledTask
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger _logger;

        public ScheduleTask(IConfigurationManager configurationManager, ILogger logger, ILibraryManager libraryManager)
        {
            _configurationManager = configurationManager;
            _logger = logger;
            _libraryManager = libraryManager;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerDaily,
                    TimeOfDayTicks = TimeSpan.FromMinutes(120).Ticks,
                    MaxRuntimeTicks = TimeSpan.FromMinutes(90).Ticks
                }
            };
        }

        public string Name => "Calculate Statistics";
        public string Key => "EmbyStatCalculateStatistics";
        public string Description => "Scheduled task that will calculate statistics for EmbyStat";
        public string Category => "EmbyStat";

        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var configuration = _configurationManager.GetEmbyStatConfiguration();
            var libraryTypesToScan = configuration.LibraryTypesToScan;

            if (!libraryTypesToScan.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var libraryType in libraryTypesToScan)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromCanceled(cancellationToken);
                }

                _logger.Info($"Processing {libraryType}");

                var libraries = _libraryManager.RootFolder
                    .GetChildren(new InternalItemsQuery())
                    .Where(x => _libraryManager.GetLibraryOptions(x).ContentType == libraryType)
                    .ToArray();

                if (libraryType == "movies")
                {
                    var calculator = new MoviesCalculator(_logger, _libraryManager);
                    var result = calculator.CalculateStatistics(libraries);
                }

                configuration.LibraryTypesToScan = configuration.LibraryTypesToScan.Where(x => x != libraryType).ToList();
                _configurationManager.SaveEmbyStatConfiguration(configuration);
            }

            progress.Report(100);
            return Task.CompletedTask;
        }
    }
}
