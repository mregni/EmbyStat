using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly LiteCollection<Job> _jobCollection;

        public JobRepository(IDbContext context)
        {
            _jobCollection = context.GetContext().GetCollection<Job>();
        }

        public IEnumerable<Job> GetAll()
        {
            return _jobCollection.FindAll();
        }

        public Job GetById(Guid id)
        {
            return _jobCollection.FindById(id);
        }

        public void StartJob(Job job)
        {
            var obj = _jobCollection.FindById(job.Id);

            if (obj != null)
            {
                obj.State = job.State;
                obj.StartTimeUtc = job.StartTimeUtc;
                obj.EndTimeUtc = job.EndTimeUtc;
                obj.CurrentProgressPercentage = job.CurrentProgressPercentage;
                _jobCollection.Update(obj);
            }
        }

        public void EndJob(Guid id, DateTime endTime, JobState state)
        {
            var obj = _jobCollection.FindById(id);

            if (obj != null)
            {
                obj.EndTimeUtc = endTime;
                obj.State = state;
                obj.CurrentProgressPercentage = 100;
                _jobCollection.Update(obj);
            }
        }

        public bool UpdateTrigger(Guid id, string trigger)
        {
            var obj = _jobCollection.FindById(id);

            if (obj != null)
            {
                obj.Trigger = trigger;
                _jobCollection.Update(obj);
                return true;
            }

            return false;
        }

        public void ResetAllJobs()
        {
            var jobs = _jobCollection.FindAll().ToList();
            jobs.ForEach(x =>
            {
                if (x.State == JobState.Running)
                {
                    x.State = JobState.Failed;
                    x.EndTimeUtc = DateTime.Now;
                }
            });

            _jobCollection.Update(jobs);
        }
    }
}
