using System;
using System.Threading.Tasks;
using AutoMapper.Mappers;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
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
        protected readonly IJobHubHelper _hubHelper;
        private readonly IJobRepository _jobRepository;
        private readonly IConfigurationService _configurationService;
        private JobState State { get; set; }
        private DateTime? StartTimeUtc { get; set; }

        protected BaseJob(IJobHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService)
        {
            _hubHelper = hubHelper;
            _jobRepository = jobRepository;
            _configurationService = configurationService;
        }

        public abstract Guid Id { get; }
        public abstract string JobPrefix { get; }
        public abstract string Title { get; }
        public abstract Task RunJob();
        public abstract void OnFail();
        public abstract void Dispose();

        public async Task Execute()
        {
            try
            {
                PreJobExecution();
                await RunJob();
                PostJobExecution();
            }
            catch (WizardNotFinishedException e)
            {
                LogWarning(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while running job");
                OnFail();
                FailExecution(string.Empty);
                throw;
            }
        }

        private void PreJobExecution()
        {
            if (!_configurationService.GetServerSettings().WizardFinished)
            {
                throw new WizardNotFinishedException("Job not running because wizard is not finished");
            }

            State = JobState.Running;
            StartTimeUtc = DateTime.UtcNow;
            var job = new Job{CurrentProgressPercentage = 0, Id = Id, State = State, StartTimeUtc = StartTimeUtc, EndTimeUtc = null};

            _jobRepository.StartJob(job);
            SendLogProgressToFront(0);
            LogInformation("Starting job");
        }

        private void PostJobExecution()
        {
            var now = DateTime.UtcNow;
            State = JobState.Completed;
            _jobRepository.EndJob(Id, now , State);
            SendLogProgressToFront(100, now);

            var runTime = now.Subtract(StartTimeUtc ?? now).TotalMinutes;
            LogInformation(Math.Ceiling(runTime) == 1
                ? "Job finished after 1 minute."
                : $"Job finished after {Math.Ceiling(runTime)} minutes.");
        }

        private void FailExecution(string message)
        {
            State = JobState.Failed;
            _jobRepository.EndJob(Id, DateTime.UtcNow, State);
            if (!string.IsNullOrWhiteSpace(message))
            {
                LogError(message);
            }
            SendLogProgressToFront(100);
        }
        
        public void LogProgress(double progress)
        {
            SendLogProgressToFront(progress);
        }

        public void LogInformation(string message)
        {
            Log.Information($"{JobPrefix}\t{message}");
            SendLogUpdateToFront(message, ProgressLogType.Information);
        }

        public void LogWarning(string message)
        {
            Log.Warning($"{JobPrefix}\t{message}");
            SendLogUpdateToFront(message, ProgressLogType.Warning);
        }

        public void LogError(string message)
        {
            Log.Error($"{JobPrefix}\t{message}");
            SendLogUpdateToFront(message, ProgressLogType.Error);
        }

        private async void SendLogUpdateToFront(string message, ProgressLogType type)
        {
            await _hubHelper.BroadCastJobLog(JobPrefix, message, type);
        }

        private async void SendLogProgressToFront(double progress, DateTime? EndTimeUtc = null)
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
            await _hubHelper.BroadcastJobProgress(info);
        }
    }
}