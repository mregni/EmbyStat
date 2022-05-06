using System;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs;

[Queue("main")]
public abstract class BaseJob : IBaseJob, IDisposable
{
    protected readonly IHubHelper HubHelper;
    private readonly IJobRepository _jobRepository;
    private bool _disposed;
    protected ILogger<BaseJob> Logger;

    private JobState State { get; set; }
    private DateTime? StartTimeUtc { get; }
    protected UserSettings Settings { get; }
    private bool EnableUiLogging { get; }
    private double Progress { get; set; }

    protected BaseJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
        bool enableUiLogging, ILogger<BaseJob> logger)
    {
        HubHelper = hubHelper;
        _jobRepository = jobRepository;
        Settings = settingsService.GetUserSettings();
        EnableUiLogging = enableUiLogging;
        Progress = 0;
        StartTimeUtc = DateTime.UtcNow;
        Logger = logger;
    }

    protected BaseJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, ILogger<BaseJob> logger)
        :this(hubHelper, jobRepository, settingsService, true, logger)
    {
            
    }

    protected abstract Guid Id { get; }
    protected abstract string JobPrefix { get; }
    protected abstract Task RunJobAsync();

    public async Task Execute()
    {
        try
        {
            await PreJobExecution();
            await RunJobAsync();
            await PostJobExecution();
        }
        catch (WizardNotFinishedException e)
        {
            await LogWarning(e.Message);
            await FailExecution();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error while running job");
            await FailExecution(e.Message);
            await FailExecution("Job failed, check logs for more info.");
            throw;
        }
    }

    private async Task PreJobExecution()
    {
        if (!Settings.WizardFinished)
        {
            throw new WizardNotFinishedException("Job not running because wizard is not finished");
        }

        State = JobState.Running;
        
        await BroadcastProgress(0);
        await LogInformation("Starting job");
        await _jobRepository.StartJob(Id);
    }

    private async Task PostJobExecution()
    {
        var now = DateTime.UtcNow;
        State = JobState.Completed;
        await _jobRepository.EndJob(Id, now, State);
        await BroadcastProgress(100, now);

        var runTime = now.Subtract(StartTimeUtc ?? now).TotalMinutes;
        await LogInformation(Math.Abs(Math.Ceiling(runTime) - 1) < 0.1
            ? "Job finished after 1 minute."
            : $"Job finished after {Math.Ceiling(runTime)} minutes.");
    }

    private async Task FailExecution()
    {
        await FailExecution(string.Empty);
    }

    private async Task FailExecution(string message)
    {
        var now = DateTime.UtcNow;
        State = JobState.Failed;
        await _jobRepository.EndJob(Id, now, State);
        if (!string.IsNullOrWhiteSpace(message))
        {
            await LogError(message);
        }
        await BroadcastProgress(100, now);
    }

    protected async Task LogProgressIncrement(double increment)
    {
        Progress += increment;
        await BroadcastProgress(Math.Floor(Progress * 10) / 10);
    }

    protected async Task LogProgress(double increment)
    {
        Progress = increment;
        await BroadcastProgress(Progress);
    }

    protected async Task LogInformation(string message)
    {
        if (EnableUiLogging)
        {
            Logger.LogInformation(message);
            await SendLogUpdateToFront(message, ProgressLogType.Information);
        }
    }

    protected async Task LogWarning(string message)
    {
        Logger.LogWarning(message);
        await SendLogUpdateToFront(message, ProgressLogType.Warning);
    }

    protected async Task LogError(string message)
    {
        Logger.LogError(message);
        await SendLogUpdateToFront(message, ProgressLogType.Error);
    }

    private async Task SendLogUpdateToFront(string message, ProgressLogType type)
    {
        await HubHelper.BroadcastJobLog(JobPrefix, message, type);
    }

    private async Task BroadcastProgress(double progress, DateTime? endTimeUtc = null)
    {
        var info = new JobProgress
        {
            Id = Id,
            CurrentProgressPercentage = progress,
            State = State,
            StartTimeUtc = StartTimeUtc ?? DateTime.UtcNow,
            EndTimeUtc = endTimeUtc
        };
        await HubHelper.BroadcastJobProgress(info);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~BaseJob()
    {
        Dispose(false);
    }
}