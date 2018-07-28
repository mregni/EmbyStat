using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EmbyStat.Tasks.Tasks
{
    public class DatabaseCleanupTask : IScheduledTask
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IGenreRepository _genreRepository;
        public DatabaseCleanupTask(IApplicationBuilder app)
        {
            _statisticsRepository = app.ApplicationServices.GetService<IStatisticsRepository>();
            _personRepository = app.ApplicationServices.GetService<IPersonRepository>();
            _genreRepository = app.ApplicationServices.GetService<IGenreRepository>();
        }

        public string Name => "TASKS.DATABASECLEANUP";
        public string Key => "DatabaseCleanup";
        public string Description => "TASKS.DATABASECLEANUPDESCRIPTION";
        public string Category => "Emby";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger progressLogger)
        {
            progress.Report(0);
            progressLogger.LogInformation(Constants.LogPrefix.DatabaseCleanupTask, "Start cleaning up database");

            await _statisticsRepository.CleanupStatistics();
            progress.Report(33);

            await _personRepository.CleanupPersons();
            progress.Report(66);

            await _genreRepository.CleanupGenres();
            progressLogger.LogInformation(Constants.LogPrefix.DatabaseCleanupTask, "Cleaning up database finished");
            progress.Report(100);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ TaskKey = Key, TimeOfDayTicks = 0, Type = "DailyTrigger"}
            };
        }
    }
}
