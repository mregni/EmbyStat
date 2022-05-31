using System;
using EmbyStat.Common;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs;

public class JobInitializer : IJobInitializer
{
    private readonly IDatabaseCleanupJob _databaseCleanupJob;
    private readonly IPingEmbyJob _pingEmbyJob;
    private readonly IShowSyncJob _showSyncJob;
    private readonly ISmallSyncJob _smallSyncJob;
    private readonly ICheckUpdateJob _checkUpdateJob;
    private readonly IJobService _jobService;
    private readonly IMovieSyncJob _movieSyncJob;
    private readonly IRecurringJobManager _recurringJobManager;

    public JobInitializer(IDatabaseCleanupJob databaseCleanupJob, IPingEmbyJob pingEmbyJob, IShowSyncJob showSyncJob,
        ISmallSyncJob smallSyncJob, ICheckUpdateJob checkUpdateJob, IJobService jobService, IRecurringJobManager recurringJobManager, IMovieSyncJob movieSyncJob)
    {
        _databaseCleanupJob = databaseCleanupJob;
        _pingEmbyJob = pingEmbyJob;
        _showSyncJob = showSyncJob;
        _smallSyncJob = smallSyncJob;
        _checkUpdateJob = checkUpdateJob;
        _jobService = jobService;
        _recurringJobManager = recurringJobManager;
        _movieSyncJob = movieSyncJob;
    }

    public void Setup(bool disableUpdates)
    {
        var jobs = _jobService.GetAll();
        foreach (var job in jobs)
        {
            UpdateTrigger(job.Id, job.Trigger, disableUpdates);
        }
    }

    public void UpdateTrigger(Guid id, string trigger, bool disableUpdates)
    {
        if (id == Constants.JobIds.ShowSyncId)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _showSyncJob.Execute(), trigger);
        }
        else if (id == Constants.JobIds.MovieSyncId)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _movieSyncJob.Execute(), trigger);
        }
        else if (id == Constants.JobIds.CheckUpdateId && !disableUpdates)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _checkUpdateJob.Execute(), trigger);
        }
        else if (id == Constants.JobIds.DatabaseCleanupId)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _databaseCleanupJob.Execute(), trigger);
        }
        else if (id == Constants.JobIds.PingEmbyId)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _pingEmbyJob.Execute(), trigger);
        }
        else if (id == Constants.JobIds.SmallSyncId)
        {
            _recurringJobManager.AddOrUpdate(id.ToString(), () => _smallSyncJob.Execute(), trigger);
        }
    }
}