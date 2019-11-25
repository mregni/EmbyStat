using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Jobs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Hangfire;
using Hangfire.Storage;
using Moq;
using Xunit;
using Hangfire.MemoryStorage;
using Job = EmbyStat.Common.Models.Entities.Job;

namespace Tests.Unit.Jobs
{
    public class JobInitializerTests
    {
        private readonly JobInitializer _jobInitializer;
        public JobInitializerTests()
        {
            var databaseCleanupJobMock = new Mock<IDatabaseCleanupJob>();
            var pingEmbyJobMock = new Mock<IPingEmbyJob>();
            var mediaSyncJobMock = new Mock<IMediaSyncJob>();
            var smallSyncJobMock = new Mock<ISmallSyncJob>();
            var checkUpdateJobMock = new Mock<ICheckUpdateJob>();
            
            var jobList = new List<Job>
            {
                new Job {Id = Constants.JobIds.MediaSyncId, Trigger = "0 2 * * 1"},
                new Job {Id = Constants.JobIds.CheckUpdateId, Trigger = "0 2 * * 1"},
                new Job {Id = Constants.JobIds.DatabaseCleanupId, Trigger = "0 2 * * 1"},
                new Job {Id = Constants.JobIds.PingEmbyId, Trigger = "0 2 * * 1"},
                new Job {Id = Constants.JobIds.SmallSyncId, Trigger = "0 2 * * 1"}
            };
            var jobServiceMock = new Mock<IJobService>();
            jobServiceMock.Setup(x => x.GetAll()).Returns(jobList);
            _jobInitializer = new JobInitializer(databaseCleanupJobMock.Object, pingEmbyJobMock.Object, mediaSyncJobMock.Object, smallSyncJobMock.Object, checkUpdateJobMock.Object, jobServiceMock.Object);
        }

        [Fact]
        public void Setup_Should_Enable_All_Jobs()
        {
            JobStorage.Current = new MemoryStorage();

            _jobInitializer.Setup(false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            jobs.Count.Should().Be(5);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.CheckUpdateId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.DatabaseCleanupId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.MediaSyncId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.PingEmbyId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.SmallSyncId.ToString()).Should().NotBeNull();
        }

        [Fact]
        public void Setup_Should_Enable_All_Jobs_But_Update_Job()
        {
            JobStorage.Current = null;
            JobStorage.Current = new MemoryStorage();

            _jobInitializer.Setup(true);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            jobs.Count.Should().Be(4);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.CheckUpdateId.ToString()).Should().BeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.DatabaseCleanupId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.MediaSyncId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.PingEmbyId.ToString()).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.SmallSyncId.ToString()).Should().NotBeNull();
        }

        [Fact]
        public void UpdateTrigger_Should_Update_MediaSync_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();
            RecurringJob.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            _jobInitializer.UpdateTrigger(Constants.JobIds.MediaSyncId, "0 3 * * 1", false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(1);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.MediaSyncId.ToString()).Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.MediaSyncId.ToString()).Cron.Should().Be("0 3 * * 1");
        }

        [Fact]
        public void UpdateTrigger_Should_Update_SmallSync_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();
            RecurringJob.AddOrUpdate(Constants.JobIds.SmallSyncId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            _jobInitializer.UpdateTrigger(Constants.JobIds.SmallSyncId, "0 3 * * 1", false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(1);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.SmallSyncId.ToString()).Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.SmallSyncId.ToString()).Cron.Should().Be("0 3 * * 1");
        }

        [Fact]
        public void UpdateTrigger_Should_Update_DatabaseCleanup_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();
            RecurringJob.AddOrUpdate(Constants.JobIds.DatabaseCleanupId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            _jobInitializer.UpdateTrigger(Constants.JobIds.DatabaseCleanupId, "0 3 * * 1", false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(1);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.DatabaseCleanupId.ToString()).Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.DatabaseCleanupId.ToString()).Cron.Should().Be("0 3 * * 1");
        }

        [Fact]
        public void UpdateTrigger_Should_Update_PingEmby_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();
            RecurringJob.AddOrUpdate(Constants.JobIds.PingEmbyId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            _jobInitializer.UpdateTrigger(Constants.JobIds.PingEmbyId, "0 3 * * 1", false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(1);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.PingEmbyId.ToString()).Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.PingEmbyId.ToString()).Cron.Should().Be("0 3 * * 1");
        }

        [Fact]
        public void UpdateTrigger_Should_Update_CheckUpdate_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();
            RecurringJob.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            _jobInitializer.UpdateTrigger(Constants.JobIds.CheckUpdateId, "0 3 * * 1", false);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(1);
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.CheckUpdateId.ToString()).Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.SingleOrDefault(x => x.Id == Constants.JobIds.CheckUpdateId.ToString()).Cron.Should().Be("0 3 * * 1");
        }

        [Fact]
        public void UpdateTrigger_Should_Not_Update_CheckUpdate_Job_Trigger()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();

            _jobInitializer.UpdateTrigger(Constants.JobIds.CheckUpdateId, "0 3 * * 1", true);
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            jobs.Count.Should().Be(0);
        }

        private void DeleteJobs()
        {
            RecurringJob.RemoveIfExists(Constants.JobIds.SmallSyncId.ToString());
            RecurringJob.RemoveIfExists(Constants.JobIds.CheckUpdateId.ToString());
            RecurringJob.RemoveIfExists(Constants.JobIds.DatabaseCleanupId.ToString());
            RecurringJob.RemoveIfExists(Constants.JobIds.MediaSyncId.ToString());
            RecurringJob.RemoveIfExists(Constants.JobIds.PingEmbyId.ToString());
        }

        public static string DummyCall()
        {
            return "test fummy function for hangfire jobs";
        }
    }
}
