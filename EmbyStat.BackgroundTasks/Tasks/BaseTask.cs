using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.BackgroundTasks.Interfaces;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Logging;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.BackgroundTasks.Tasks
{
    public abstract class BaseTask: IBackgroundTask, IDisposable
    {
        public abstract TimeSpan GetTrigger();
        protected abstract Task RunMainJob(CancellationToken token);
        public abstract Guid Id { get; }
        public abstract string Title { get; }
        public abstract string JobPrefix { get; }
        private bool _disposed;

        public AutoResetEvent GetAutoEvent() => _autoEvent;
        private readonly AutoResetEvent _autoEvent;
        protected readonly Logger Logger;
        private JobState State { get; set; }
        private DateTime? StartTimeUtc { get; set; }
        protected UserSettings Settings { get; set; }
        protected bool EnableUiLogging { get; set; }
        private double Progress { get; set; }
        protected readonly IJobHubHelper HubHelper;
        protected readonly ISettingsService SettingsService;
        private readonly IJobRepository _jobRepository;

        protected BaseTask(IJobHubHelper hubHelper, ISettingsService settingsService, IJobRepository jobRepository, Type type, string prefix)
        {
            HubHelper = hubHelper;
            SettingsService = settingsService;
            _jobRepository = jobRepository;

            Settings = settingsService.GetUserSettings();
            _autoEvent = new AutoResetEvent(false);
            Logger = LogFactory.CreateLoggerForType(type, prefix);
            Progress = 0;
        }

        public async Task RunJob(CancellationToken token)
        {
            await PreTaskExecution();
            await RunMainJob(token);
            await PostTaskExecution();
        }

        private async Task PreTaskExecution()
        {
            if (!Settings.WizardFinished)
            {
                throw new WizardNotFinishedException("Job not running because wizard is not finished");
            }

            await BroadcastProgress(0);
            await LogInformation("Starting job");

            State = JobState.Running;
            StartTimeUtc = DateTime.UtcNow;
            var job = new Job { CurrentProgressPercentage = 0, Id = Id, State = State, StartTimeUtc = StartTimeUtc, EndTimeUtc = null };

            _jobRepository.StartJob(job);
        }

        private async Task PostTaskExecution()
        {
            var now = DateTime.UtcNow;
            State = JobState.Completed;
            _jobRepository.EndJob(Id, now, State);
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
            _jobRepository.EndJob(Id, now, State);
            if (!string.IsNullOrWhiteSpace(message))
            {
                await LogError(message);
            }
            await BroadcastProgress(100, now);
        }

        public async Task LogProgressIncrement(double increment)
        {
            Progress += increment;
            await BroadcastProgress(Math.Floor(Progress * 10) / 10);
        }

        public async Task LogProgress(double increment)
        {
            Progress = increment;
            await BroadcastProgress(Progress);
        }

        public async Task LogInformation(string message)
        {
            if (EnableUiLogging)
            {
                Logger.Info(message);
                await SendLogUpdateToFront(message, ProgressLogType.Information);
            }
        }

        public async Task LogWarning(string message)
        {
            Logger.Warn(message);
            await SendLogUpdateToFront(message, ProgressLogType.Warning);
        }

        public async Task LogError(string message)
        {
            Logger.Error(message);
            await SendLogUpdateToFront(message, ProgressLogType.Error);
        }

        private async Task SendLogUpdateToFront(string message, ProgressLogType type)
        {
            await HubHelper.BroadCastJobLog(JobPrefix, message, type);
        }

        private async Task BroadcastProgress(double progress, DateTime? endTimeUtc = null)
        {
            var info = new JobProgress
            {
                Id = Id,
                CurrentProgressPercentage = progress,
                State = State,
                StartTimeUtc = StartTimeUtc ?? DateTime.UtcNow,
                EndTimeUtc = endTimeUtc,
                Title = Title
            };
            await HubHelper.BroadcastJobProgress(info);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        public void Dispose()
        {
            _autoEvent?.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
