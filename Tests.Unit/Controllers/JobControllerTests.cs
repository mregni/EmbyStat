using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Configuration;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Job;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Jobs;
using FluentAssertions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers;

public class JobControllerTests
{
    private Mock<IJobService> _jobServiceMock;
    private Mock<IJobInitializer> _jobInitializerMock;
    private Mock<IOptions<Config>> _options;
    private Mock<IHubHelper> _jobHubHelperMock;

    private JobController CreateController(bool disableUpdateJob, params Job[] jobs)
    {
        var profiles = new MapProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
        var mapper = new Mapper(configuration);

        _jobServiceMock = new Mock<IJobService>();
        _jobInitializerMock = new Mock<IJobInitializer>();
        _options = new Mock<IOptions<Config>>();
        _jobHubHelperMock = new Mock<IHubHelper>();

        _jobServiceMock.Setup(x => x.GetAll()).Returns(jobs);
        foreach (var job in jobs)
        {
            _jobServiceMock.Setup(x => x.GetById(job.Id)).Returns(job);
            _jobServiceMock.Setup(x => x.UpdateTrigger(job.Id, It.IsAny<string>())).ReturnsAsync(true);
        }

        _jobHubHelperMock.Setup(x =>
            x.BroadcastJobLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProgressLogType>()));
        _jobInitializerMock.Setup(x => x.UpdateTrigger(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()));

        _options
            .Setup(x => x.Value)
            .Returns(new Config
            {
                SystemConfig = new SystemConfig()
                {
                    UpdatesDisabled = disableUpdateJob
                }
            });

        return new JobController(mapper, _jobServiceMock.Object, _jobHubHelperMock.Object,
            _jobInitializerMock.Object, _options.Object);
    }

    [Fact]
    public void GetAll_Should_Return_All_Jobs()
    {
        var jobOne = new Job {Id = Guid.NewGuid()};
        var jobTwo = new Job {Id = Guid.NewGuid()};
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

        _options.Verify(x => x.Value, Times.Once);
        _jobServiceMock.Verify(x => x.GetAll(), Times.Once);
            
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetAll_Should_Return_All_Jobs_Except_Update_Job()
    {
        var jobOne = new Job {Id = Guid.NewGuid()};
        var jobTwo = new Job {Id = Guid.NewGuid()};
        var jobThree = new Job {Id = Constants.JobIds.CheckUpdateId};
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

        _options.Verify(x => x.Value, Times.Once);
        _jobServiceMock.Verify(x => x.GetAll(), Times.Once);
            
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetById_Should_Return_Correct_Job()
    {
        var jobOne = new Job {Id = Guid.NewGuid()};
        var jobTwo = new Job {Id = Guid.NewGuid()};
        var controller = CreateController(true, jobOne, jobTwo);

        var result = controller.Get(jobTwo.Id);
        var jobObject = result as OkObjectResult;
        jobObject.Should().NotBeNull();

        // ReSharper disable once PossibleNullReferenceException
        var job = jobObject.Value as JobViewModel;
        job.Should().NotBeNull();
        // ReSharper disable once PossibleNullReferenceException
        job.Id.Should().Be(jobTwo.Id);
            
        _jobServiceMock.Verify(x => x.GetById(jobTwo.Id), Times.Once);
        
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetById_Should_Return_NotFound_If_Job_Is_Not_Found()
    {
        var jobOne = new Job {Id = Guid.NewGuid()};
        var jobTwo = new Job {Id = Guid.NewGuid()};
        var controller = CreateController(true, jobOne, jobTwo);

        var randomId = Guid.NewGuid();
        var result = controller.Get(randomId);

        var res = result as NotFoundResult;
        res.Should().NotBeNull();
        res!.StatusCode.Should().Be((int) HttpStatusCode.NotFound);

        _jobServiceMock.Verify(x => x.GetById(randomId), Times.Once);
        
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateTrigger_Should_Update_Correct_Job()
    {
        var jobOne = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var jobTwo = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var controller = CreateController(false, jobOne, jobTwo);

        var result = await controller.UpdateTrigger(jobOne.Id, "* * * 1 0");

        var res = result as NoContentResult;
        res.Should().NotBeNull();
        res!.StatusCode.Should().Be((int) HttpStatusCode.NoContent);

        _options.Verify(x => x.Value, Times.Once);
        _jobServiceMock.Verify(x => x.UpdateTrigger(jobOne.Id, "* * * 1 0"), Times.Once);
        _jobInitializerMock.Verify(x => x.UpdateTrigger(jobOne.Id, "* * * 1 0", false));
            
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateTrigger_Should_Return_Not_found_If_Job_Is_Not_Found()
    {
        var jobOne = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var jobTwo = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var controller = CreateController(false, jobOne, jobTwo);

        var randomId = Guid.NewGuid();
        var result = await controller.UpdateTrigger(randomId, "* * * 1 0");

        var res = result as NotFoundResult;
        res.Should().NotBeNull();
        res!.StatusCode.Should().Be((int) HttpStatusCode.NotFound);

        _jobServiceMock.Verify(x => x.UpdateTrigger(randomId, "* * * 1 0"), Times.Once);
        
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FireJob_Should_Fire_Correct_Job()
    {
        JobStorage.Current = new MemoryStorage();
        DeleteJobs();

        RecurringJob.AddOrUpdate(Constants.JobIds.ShowSyncId.ToString(),
            () => DummyCall(),
            "0 2 * * 1");

        var jobOne = new Job {Id = Constants.JobIds.ShowSyncId, Trigger = "* * * * *"};
        var jobTwo = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var controller = CreateController(false, jobOne, jobTwo);

        var result = await controller.FireJob(jobOne.Id);

        var res = result as OkResult;
        res.Should().NotBeNull();
        res!.StatusCode.Should().Be((int) HttpStatusCode.OK);

        _jobServiceMock.Verify(x => x.GetById(jobOne.Id), Times.Once);
        _jobHubHelperMock.Verify(
            x => x.BroadcastJobLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProgressLogType>()),
            Times.Once);
            
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FireJob_Should_Return_NotFound_If_Job_Is_Not_Found()
    {
        var jobOne = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var jobTwo = new Job {Id = Guid.NewGuid(), Trigger = "* * * * *"};
        var controller = CreateController(false, jobOne, jobTwo);

        var randomId = Guid.NewGuid();
        var result = await controller.FireJob(randomId);

        var res = result as NotFoundResult;
        res.Should().NotBeNull();
        res!.StatusCode.Should().Be((int) HttpStatusCode.NotFound);

        _jobServiceMock.Verify(x => x.GetById(randomId), Times.Once);
        
        _options.Verify(x => x.Value, Times.Once);
        _options.VerifyNoOtherCalls();
        _jobServiceMock.VerifyNoOtherCalls();
        _jobInitializerMock.VerifyNoOtherCalls();
    }

    private void DeleteJobs()
    {
        RecurringJob.RemoveIfExists(Constants.JobIds.ShowSyncId.ToString());
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string DummyCall()
    {
        return "test fummy function for hangfire jobs";
    }
}