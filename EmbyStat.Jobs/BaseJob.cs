using System;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Serilog;

namespace EmbyStat.Jobs
{
    [Queue("main")]
    public abstract class BaseJob : IBaseJob
    {
        protected readonly IJobHubHelper HubHelper;
        protected readonly ISettingsService SettingsService;
        private readonly IJobRepository _jobRepository;
        private JobState State { get; set; }
        private DateTime? StartTimeUtc { get; set; }
        protected UserSettings Settings { get; set; }

        protected BaseJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService)
        {
            HubHelper = hubHelper;
            _jobRepository = jobRepository;
            Settings = settingsService.GetUserSettings();
            SettingsService = settingsService;
        }

        public abstract Guid Id { get; }
        public abstract string JobPrefix { get; }
        public abstract string Title { get; }
        public abstract Task RunJob();

        public async Task Execute()
        {
            try
            {
                await PreJobExecution();
                await RunJob();
                await PostJobExecution();
            }
            catch (WizardNotFinishedException e)
            {
                await LogWarning(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while running job");
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
            StartTimeUtc = DateTime.UtcNow;
            var job = new Job{CurrentProgressPercentage = 0, Id = Id, State = State, StartTimeUtc = StartTimeUtc, EndTimeUtc = null};

            _jobRepository.StartJob(job);
            await SendLogProgressToFront(0);
            await LogInformation("Starting job");
        }

        private async Task PostJobExecution()
        {
            var now = DateTime.UtcNow;
            State = JobState.Completed;
            _jobRepository.EndJob(Id, now , State);
            await SendLogProgressToFront(100, now);

            var runTime = now.Subtract(StartTimeUtc ?? now).TotalMinutes;
            await LogInformation(Math.Ceiling(runTime) == 1
                ? "Job finished after 1 minute."
                : $"Job finished after {Math.Ceiling(runTime)} minutes.");
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
            await SendLogProgressToFront(100, now);
        }
        
        public async Task LogProgress(double progress)
        {
            await SendLogProgressToFront(progress);
        }

        public async Task LogInformation(string message)
        {
            Log.Information($"{JobPrefix}\t{message}");
            await SendLogUpdateToFront(message, ProgressLogType.Information);
        }

        public async Task LogWarning(string message)
        {
            Log.Warning($"{JobPrefix}\t{message}");
            await SendLogUpdateToFront(message, ProgressLogType.Warning);
        }

        public async Task LogError(string message)
        {
            Log.Error($"{JobPrefix}\t{message}");
            await SendLogUpdateToFront(message, ProgressLogType.Error);
        }

        private async Task SendLogUpdateToFront(string message, ProgressLogType type)
        {
            await HubHelper.BroadCastJobLog(JobPrefix, message, type);
        }

        private async Task SendLogProgressToFront(double progress, DateTime? EndTimeUtc = null)
        {
            var info = new JobProgress
            {
                Id = Id,
                CurrentProgressPercentage = progress,
                State = State,
                StartTimeUtc = StartTimeUtc ?? DateTime.UtcNow,
                EndTimeUtc = EndTimeUtc,
                Title = Title
            };
            await HubHelper.BroadcastJobProgress(info);
        }
    }
}