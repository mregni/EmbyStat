using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Maintenance;

[DisableConcurrentExecution(5 * 60)]
public class DatabaseCleanupJob : BaseJob, IDatabaseCleanupJob
{
    private readonly IStatisticsRepository _statisticsRepository;

    public DatabaseCleanupJob(IHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService,
        IStatisticsRepository statisticsRepository, ILogger<DatabaseCleanupJob> logger)
        : base(hubHelper, jobRepository, configurationService, logger)
    {
        _statisticsRepository = statisticsRepository;
    }

    protected sealed override Guid Id => Constants.JobIds.DatabaseCleanupId;
    protected override string JobPrefix => Constants.LogPrefix.DatabaseCleanupJob;

    protected override async Task RunJobAsync()
    {
        await _statisticsRepository.DeleteStatistics();
        await LogProgress(50);
        await LogInformation("Removed old statistic results.");
    }
}