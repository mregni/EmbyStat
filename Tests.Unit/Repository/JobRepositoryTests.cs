using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Repository
{
    public class JobRepositoryTests : BaseRepositoryTester
    {
        private JobRepository _jobRepository;
        private DbContext _context;
        public JobRepositoryTests() : base("test-data-job-repo.db")
        {
        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _jobRepository = new JobRepository(_context);
        }

        [Fact]
        public void GetAll_Should_Return_All_Jobs()
        {
            RunTest(() =>
            {
                var jobOne = new Job {Id = Guid.NewGuid()};
                var jobTwo = new Job {Id = Guid.NewGuid()};
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] {jobOne, jobTwo});
                }

                var jobs = _jobRepository.GetAll();
                jobs.Count.Should().Be(2);

                jobs.Count(x => x.Id == jobOne.Id).Should().Be(1);
                jobs.Count(x => x.Id == jobTwo.Id).Should().Be(1);
            });
        }

        [Fact]
        public void GetById_Should_Return_Correct_Job()
        {
            RunTest(() =>
            {
                var jobOne = new Job { Id = Guid.NewGuid() };
                var jobTwo = new Job { Id = Guid.NewGuid() };
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] { jobOne, jobTwo });
                }

                var job = _jobRepository.GetById(jobTwo.Id);
                job.Should().NotBeNull();
                job.Id.Should().Be(jobTwo.Id);
            });
        }

        [Fact]
        public void StartJob_Should_Start_Correct_Job()
        {
            RunTest(() =>
            {
                var jobOne = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };
                var jobTwo = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] { jobOne, jobTwo });
                }

                jobOne.CurrentProgressPercentage = 10;
                jobOne.State = JobState.Running;
                jobOne.StartTimeUtc = DateTime.Now;
                
                _jobRepository.StartJob(jobOne);

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    var job = collection.FindById(jobOne.Id);

                    job.Should().NotBeNull();
                    job.Id.Should().Be(jobOne.Id);
                    job.CurrentProgressPercentage.Should().Be(10);
                    job.StartTimeUtc.Should().HaveValue();
                    // ReSharper disable once PossibleInvalidOperationException
                    job.StartTimeUtc.Value.Should().BeCloseTo(DateTime.Now, 100);
                    job.EndTimeUtc.Should().BeNull();
                    job.State.Should().Be(JobState.Running);
                }
            });
        }

        [Fact]
        public void EndJob_Should_End_Correct_Job()
        {
            RunTest(() =>
            {
                var jobOne = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };
                var jobTwo = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] { jobOne, jobTwo });
                }

                _jobRepository.EndJob(jobOne.Id, DateTime.Now, JobState.Completed);

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    var job = collection.FindById(jobOne.Id);

                    job.Should().NotBeNull();
                    job.Id.Should().Be(jobOne.Id);
                    job.CurrentProgressPercentage.Should().Be(100);
                    job.EndTimeUtc.Should().HaveValue();
                    // ReSharper disable once PossibleInvalidOperationException
                    job.EndTimeUtc.Value.Should().BeCloseTo(DateTime.Now, 500);
                    job.State.Should().Be(JobState.Completed);
                }
            });
        }

        [Fact]
        public void UpdateTrigger_Should_Update_The_Trigger_Of_One_Job()
        {
            RunTest(() =>
            {
                var jobOne = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null, Trigger = "* * * * *"};
                var jobTwo = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] { jobOne, jobTwo });
                    var job = collection.FindById(jobOne.Id);
                    job.Trigger.Should().Be("* * * * *");
                }

                var result = _jobRepository.UpdateTrigger(jobOne.Id, "* */5 * * *");
                result.Should().BeTrue();

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    var job = collection.FindById(jobOne.Id);

                    job.Should().NotBeNull();
                    job.Id.Should().Be(jobOne.Id);
                    job.Trigger.Should().Be("* */5 * * *");
                }
            });
        }

        [Fact]
        public void UpdateTrigger_Should_Return_False_When_Job_Is_Not_Found()
        {
            RunTest(() =>
            {
                var result = _jobRepository.UpdateTrigger(Guid.NewGuid(), "* */5 * * *");
                result.Should().BeFalse();
            });
        }

        [Fact]
        public void ResetJobs_Should_Fail_All_Still_Running_Jobs()
        {
            RunTest(() =>
            {
                var jobOne = new Job { Id = Guid.NewGuid(), State = JobState.Running, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };
                var jobTwo = new Job { Id = Guid.NewGuid(), State = JobState.Idle, StartTimeUtc = null, EndTimeUtc = null, CurrentProgressPercentage = null };

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    collection.InsertBulk(new[] { jobOne, jobTwo });
                }

                _jobRepository.ResetAllJobs();

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Job>();
                    var job = collection.FindById(jobOne.Id);

                    job.Should().NotBeNull();
                    job.Id.Should().Be(jobOne.Id);
                    job.State.Should().Be(JobState.Failed);

                    job = collection.FindById(jobTwo.Id);

                    job.Should().NotBeNull();
                    job.Id.Should().Be(jobTwo.Id);
                    job.State.Should().Be(JobState.Idle);
                }
            });
        }
    }
}
