using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class JobRepository : BaseRepository, IJobRepository
    {
        public JobRepository(IDbContext context) : base(context)
        {
            
        }

        public List<Job> GetAll()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            return collection.FindAll().ToList();
        }

        public Job GetById(Guid id)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            return collection.FindById(id);
        }

        public void StartJob(Job job)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            var jobDocument = collection.FindById(job.Id);

            if (jobDocument != null)
            {
                jobDocument.State = job.State;
                jobDocument.StartTimeUtc = job.StartTimeUtc;
                jobDocument.EndTimeUtc = job.EndTimeUtc;
                jobDocument.CurrentProgressPercentage = job.CurrentProgressPercentage;
                collection.Update(jobDocument);
            }
        }

        public void EndJob(Guid id, DateTime endTime, JobState state)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            var jobDocument = collection.FindById(id);

            if (jobDocument != null)
            {
                jobDocument.EndTimeUtc = endTime;
                jobDocument.State = state;
                jobDocument.CurrentProgressPercentage = 100;
                collection.Update(jobDocument);
            }
        }

        public bool UpdateTrigger(Guid id, string trigger)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            var job = collection.FindById(id);

            if (job != null)
            {
                job.Trigger = trigger;
                collection.Update(job);
                return true;
            }

            return false;

        }

        public void ResetAllJobs()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Job>();
            var jobs = collection.FindAll().ToList();
            jobs.ForEach(x =>
            {
                if (x.State == JobState.Running)
                {
                    x.State = JobState.Failed;
                    x.EndTimeUtc = DateTime.Now;
                }
            });

            collection.Update(jobs);
        }
    }
}
