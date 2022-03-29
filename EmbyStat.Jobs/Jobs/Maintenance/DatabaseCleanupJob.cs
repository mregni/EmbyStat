using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Maintenance
{
    [DisableConcurrentExecution(5 * 60)]
    public class DatabaseCleanupJob : BaseJob, IDatabaseCleanupJob
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public DatabaseCleanupJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
            IStatisticsRepository statisticsRepository)
            : base(hubHelper, jobRepository, settingsService, typeof(DatabaseCleanupJob), Constants.LogPrefix.DatabaseCleanupJob)
        {
            _statisticsRepository = statisticsRepository;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.DatabaseCleanupId;
        public override string JobPrefix => Constants.LogPrefix.DatabaseCleanupJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            await _statisticsRepository.DeleteStatistics();
            await LogProgress(50);
            await LogInformation("Removed old statistic results.");
        }
    }
}