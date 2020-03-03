using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Jobs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Moq;
using Xunit;
using Job = EmbyStat.Common.Models.Entities.Job;

namespace Tests.Unit.Jobs
{
    public class JobInitializerTests
    {
        private readonly JobInitializer _jobInitializer;
        private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
        private readonly Mock<IDatabaseCleanupJob> _databaseCleanupJobMock;
        private readonly Mock<IPingEmbyJob> _pingEmbyJobMock;
        private readonly Mock<IMediaSyncJob> _mediaSyncJobMock;
        private readonly Mock<ISmallSyncJob> _smallSyncJobMock;
        private readonly Mock<ICheckUpdateJob> _checkUpdateJobMock;
        
        public JobInitializerTests()
        {
            _databaseCleanupJobMock = new Mock<IDatabaseCleanupJob>();
            _pingEmbyJobMock = new Mock<IPingEmbyJob>();
            _mediaSyncJobMock = new Mock<IMediaSyncJob>();
            _smallSyncJobMock = new Mock<ISmallSyncJob>();
            _checkUpdateJobMock = new Mock<ICheckUpdateJob>();
            _checkUpdateJobMock.Setup(x => x.Execute());

            _recurringJobManagerMock = new Mock<IRecurringJobManager>();
            _recurringJobManagerMock.Setup(x => x.AddOrUpdate(It.IsAny<string>(), It.IsAny<Hangfire.Common.Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()));

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
            _jobInitializer = new JobInitializer(_databaseCleanupJobMock.Object, _pingEmbyJobMock.Object, _mediaSyncJobMock.Object, _smallSyncJobMock.Object, _checkUpdateJobMock.Object, jobServiceMock.Object, _recurringJobManagerMock.Object);
        }

        [Fact]
        public void Setup_Should_Enable_All_Jobs()
        {
            _jobInitializer.Setup(false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.DatabaseCleanupId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.PingEmbyId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.SmallSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void Setup_Should_Enable_All_Jobs_Excluding_Update_Job()
        {
            _jobInitializer.Setup(true);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Never());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.DatabaseCleanupId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.PingEmbyId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.SmallSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 2 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateTrigger_Should_Update_MediaSync_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.MediaSyncId, "0 3 * * 1", false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());

        }

        [Fact]
        public void UpdateTrigger_Should_Update_SmallSync_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.SmallSyncId, "0 3 * * 1", false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.SmallSyncId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateTrigger_Should_Update_DatabaseCleanup_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.DatabaseCleanupId, "0 3 * * 1", false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.DatabaseCleanupId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateTrigger_Should_Update_PingEmby_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.PingEmbyId, "0 3 * * 1", false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.PingEmbyId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateTrigger_Should_Update_CheckUpdate_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.CheckUpdateId, "0 3 * * 1", false);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateTrigger_Should_Not_Update_CheckUpdate_Job_Trigger()
        {
            _jobInitializer.UpdateTrigger(Constants.JobIds.CheckUpdateId, "0 3 * * 1", true);
            _recurringJobManagerMock.Verify(x => x.AddOrUpdate(Constants.JobIds.CheckUpdateId.ToString(), It.IsAny<Hangfire.Common.Job>(), "0 3 * * 1", It.IsAny<RecurringJobOptions>()), Times.Never());

        }
    }
}
