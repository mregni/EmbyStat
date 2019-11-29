using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Job;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class JobControllerTests
    {
        private Mock<IJobService> _jobServiceMock;
        private Mock<IJobInitializer> _jobInitializerMock;
        private Mock<ISettingsService> _settingsServiceMock;
        private Mock<IJobHubHelper> jobHubHelperMock;

        private JobController CreateController(bool disableUpdateJob, params Job[] jobs)
        {
            var profiles = new MapProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
            var mapper = new Mapper(configuration);

            _jobServiceMock = new Mock<IJobService>();
            _jobInitializerMock = new Mock<IJobInitializer>();
            _settingsServiceMock = new Mock<ISettingsService>();
            jobHubHelperMock = new Mock<IJobHubHelper>();

            _jobServiceMock.Setup(x => x.GetAll()).Returns(jobs);
            foreach (var job in jobs)
            {
                _jobServiceMock.Setup(x => x.GetById(job.Id)).Returns(job);
                _jobServiceMock.Setup(x => x.UpdateTrigger(job.Id, It.IsAny<string>())).Returns(true);
            }

            jobHubHelperMock.Setup(x => x.BroadCastJobLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProgressLogType>()));
            _jobInitializerMock.Setup(x => x.UpdateTrigger(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()));

            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(new AppSettings { NoUpdates = disableUpdateJob });

            return new JobController(mapper, _jobServiceMock.Object, jobHubHelperMock.Object, _jobInitializerMock.Object, _settingsServiceMock.Object);
        }

        [Fact]
        public void GetAll_Should_Return_All_Jobs()
        {
            var jobOne = new Job { Id = Guid.NewGuid() };
            var jobTwo = new Job { Id = Guid.NewGuid() };
            var controller = CreateController(false, jobOne, jobTwo);

            var result = controller.GetAll();
            result.Should().BeOfType<OkObjectResult>();

            var jobsObject = result as OkObjectResult;
            jobsObject.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var jobs = jobsObject.Value as List<JobViewModel>;
            jobs.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.Count.Should().Be(2);
            jobs.SingleOrDefault(x => x.Id == jobOne.Id).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == jobTwo.Id).Should().NotBeNull();

            _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Once);
            _jobServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void GetAll_Should_Return_All_Jobs_Except_Update_Job()
        {
            var jobOne = new Job { Id = Guid.NewGuid() };
            var jobTwo = new Job { Id = Guid.NewGuid() };
            var jobThree = new Job { Id = Constants.JobIds.CheckUpdateId };
            var controller = CreateController(true, jobOne, jobTwo, jobThree);

            var result = controller.GetAll();
            result.Should().BeOfType<OkObjectResult>();

            var jobsObject = result as OkObjectResult;
            jobsObject.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var jobs = jobsObject.Value as List<JobViewModel>;
            jobs.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            jobs.Count.Should().Be(2);
            jobs.SingleOrDefault(x => x.Id == jobOne.Id).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == jobTwo.Id).Should().NotBeNull();
            jobs.SingleOrDefault(x => x.Id == jobThree.Id).Should().BeNull();

            _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Once);
            _jobServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_Should_Return_Correct_Job()
        {
            var jobOne = new Job { Id = Guid.NewGuid() };
            var jobTwo = new Job { Id = Guid.NewGuid() };
            var controller = CreateController(true, jobOne, jobTwo);

            var result = controller.Get(jobTwo.Id);
            var jobObject = result as OkObjectResult;
            jobObject.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var job = jobObject.Value as JobViewModel;
            job.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            job.Id.Should().Be(jobTwo.Id);

            _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Never);
            _jobServiceMock.Verify(x => x.GetById(jobTwo.Id), Times.Once);
        }

        [Fact]
        public void GetById_Should_Return_NotFound_If_Job_Is_Not_Found()
        {
            var jobOne = new Job { Id = Guid.NewGuid() };
            var jobTwo = new Job { Id = Guid.NewGuid() };
            var controller = CreateController(true, jobOne, jobTwo);

            var randomId = Guid.NewGuid();
            var result = controller.Get(randomId);
            
            var res = result as NotFoundResult;
            res.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            _jobServiceMock.Verify(x => x.GetById(randomId), Times.Once);
            _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Never);
            _jobServiceMock.Verify(x => x.GetById(jobTwo.Id), Times.Never);
        }

        [Fact]
        public void UpdateTrigger_Should_Update_Correct_Job()
        {
            var jobOne = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var jobTwo = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var controller = CreateController(false, jobOne, jobTwo);

            var result = controller.UpdateTrigger(jobOne.Id, "* * * 1 0");
            
            var res = result as NoContentResult;
            res.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Once);
            _jobServiceMock.Verify(x => x.UpdateTrigger(jobOne.Id, "* * * 1 0"), Times.Once);
            _jobInitializerMock.Verify(x => x.UpdateTrigger(jobOne.Id, "* * * 1 0", false));
        }

        [Fact]
        public void UpdateTrigger_Should_Return_Not_found_If_Job_Is_Not_Found()
        {
            var jobOne = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var jobTwo = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var controller = CreateController(false, jobOne, jobTwo);

            var randomId = Guid.NewGuid();
            var result = controller.UpdateTrigger(randomId, "* * * 1 0");

            var res = result as NotFoundResult;
            res.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Never);
            _jobServiceMock.Verify(x => x.UpdateTrigger(randomId, "* * * 1 0"), Times.Once);
            _jobServiceMock.Verify(x => x.UpdateTrigger(jobOne.Id, "* * * 1 0"), Times.Never);
            _jobServiceMock.Verify(x => x.UpdateTrigger(jobTwo.Id, "* * * 1 0"), Times.Never);
            _jobInitializerMock.Verify(x => x.UpdateTrigger(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task FireJob_Should_Fire_Correct_Job()
        {
            JobStorage.Current = new MemoryStorage();
            DeleteJobs();

            RecurringJob.AddOrUpdate(Constants.JobIds.MediaSyncId.ToString(),
                () => DummyCall(),
                "0 2 * * 1");

            var jobOne = new Job { Id = Constants.JobIds.MediaSyncId, Trigger = "* * * * *" };
            var jobTwo = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var controller = CreateController(false, jobOne, jobTwo);

            var result = await controller.FireJob(jobOne.Id);

            var res = result as OkResult;
            res.StatusCode.Should().Be((int)HttpStatusCode.OK);

            _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Once);
            jobHubHelperMock.Verify(x => x.BroadCastJobLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProgressLogType>()), Times.Once);
        }

        [Fact]
        public async Task FireJob_Should_Return_NotFound_If_Job_Is_Not_Found()
        {
            var jobOne = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var jobTwo = new Job { Id = Guid.NewGuid(), Trigger = "* * * * *" };
            var controller = CreateController(false, jobOne, jobTwo);

            var randomId = Guid.NewGuid();
            var result = await controller.FireJob(randomId);

            var res = result as NotFoundResult;
            res.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            _jobServiceMock.Verify(x => x.GetById(randomId), Times.Once);
            _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Never);
            jobHubHelperMock.Verify(x => x.BroadCastJobLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProgressLogType>()), Times.Never);
        }

        [Fact]
        public void GetMediaSyncJob_Should_Return_MediaSyncJob_Job()
        {
            var jobOne = new Job { Id = Constants.JobIds.MediaSyncId };
            var jobTwo = new Job { Id = Guid.NewGuid() };
            var controller = CreateController(true, jobOne, jobTwo);

            var result = controller.GetMediaSyncJob();
            var jobObject = result as OkObjectResult;
            jobObject.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            var job = jobObject.Value as JobViewModel;
            job.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            job.Id.Should().Be(jobOne.Id);

            _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Once);
            _jobServiceMock.Verify(x => x.GetById(jobTwo.Id), Times.Never);
        }

        private void DeleteJobs()
        {
            RecurringJob.RemoveIfExists(Constants.JobIds.MediaSyncId.ToString());
        }

        public static string DummyCall()
        {
            return "test fummy function for hangfire jobs";
        }
    }
}
