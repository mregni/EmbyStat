using System;
using System.Threading.Tasks;
using EmbyStat.Common.Hubs;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Tasks.Tasks.Interfaces;

namespace EmbyStat.Tasks.Tasks.Maintenance
{
    public class DatabaseCleanupTask : TaskConnector, IDatabaseCleanupTask
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IGenreRepository _genreRepository;

        public DatabaseCleanupTask(IHubHelper hubHelper, IStatisticsRepository statisticsRepository, IPersonRepository personRepository, IGenreRepository genreRepository) : base(hubHelper)
        {
            _statisticsRepository = statisticsRepository;
            _personRepository = personRepository;
            _genreRepository = genreRepository;
        }

        public override string Name => "TASKS.DATABASECLEANUP";
        public override string Key => "DatabaseCleanup";
        public override string Description => "TASKS.DATABASECLEANUPDESCRIPTION";
        public override string Category => "Emby";
        public override Guid Id => new Guid("b109ca73-0563-4062-a3e2-f7e6a00b73e9");

        public async Task Execute()
        {
            //_progress.Report(0);
            //_progressLogger.LogInformation(Constants.LogPrefix.DatabaseCleanupTask, "Start cleaning up database");

            await _statisticsRepository.CleanupStatistics();
            //_progress.Report(33);

            await _personRepository.CleanupPersons();
            //_progress.Report(66);

            await _genreRepository.CleanupGenres();
            //_progressLogger.LogInformation(Constants.LogPrefix.DatabaseCleanupTask, "Cleaning up database finished");
            //_progress.Report(100);
        }

        public void Dispose()
        {
        }
    }
}