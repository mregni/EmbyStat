using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services;

public class JobServiceTests
{
    private readonly Mock<IJobRepository> _jobRepositoryMock;
    private readonly JobService _jobService;
    private readonly List<Job> _jobs;

    public JobServiceTests()
    {
        _jobs = new List<Job>
        {
            new() {Id = Guid.NewGuid()},
            new() {Id = Guid.NewGuid()}
        };

        _jobRepositoryMock = new Mock<IJobRepository>();
        _jobRepositoryMock.Setup(x => x.GetAll()).Returns(_jobs);
        _jobRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(_jobs[0]);
        _jobRepositoryMock.Setup(x => x.UpdateTrigger(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(true);

        _jobService = new JobService(_jobRepositoryMock.Object);
    }

    [Fact]
    public void GetAllJobs()
    {
        var jobs = _jobService.GetAll().ToList();

        jobs.Should().NotBeNull();
        jobs.Count.Should().Be(2);

        jobs[0].Id.Should().Be(_jobs[0].Id);
        jobs[1].Id.Should().Be(_jobs[1].Id);

        _jobRepositoryMock.Verify(x => x.GetAll(), Times.Exactly(1));
    }

    [Fact]
    public void GetJobById()
    {
        var job = _jobService.GetById(_jobs[0].Id);

        job.Should().NotBeNull();
        job.Id.Should().Be(_jobs[0].Id);

        _jobRepositoryMock.Verify(x => x.GetById(_jobs[0].Id), Times.Exactly(1));
    }

    [Fact]
    public async Task UpdateJobTrigger()
    {
        var success = await _jobService.UpdateTrigger(_jobs[0].Id, "* * * * *");

        success.Should().BeTrue();
        _jobRepositoryMock.Verify(x => x.UpdateTrigger(_jobs[0].Id, "* * * * *"), Times.Exactly(1));
    }

    [Fact]
    public void ResetJobs()
    {
        _jobService.ResetAllJobs();

        _jobRepositoryMock.Verify(x => x.ResetAllJobs(), Times.Exactly(1));
    }
}