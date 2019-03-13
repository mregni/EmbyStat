using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Job> GetAll()
        {
            return _context.Jobs.ToList();
        }

        public Job GetById(Guid id)
        {
            return _context.Jobs.Single(x => x.Id == id);
        }

        public void StartJob(Job job)
        {
            var obj = _context.Jobs.Single(x => x.Id == job.Id);

            if (obj != null)
            {
                obj.State = job.State;
                obj.StartTimeUtc = job.StartTimeUtc;
                obj.EndTimeUtc = job.EndTimeUtc;
                obj.CurrentProgressPercentage = job.CurrentProgressPercentage;
                _context.SaveChanges();
            }
        }

        public void EndJob(Guid id, DateTime endTime, JobState state)
        {
            var obj = _context.Jobs.Single(x => x.Id == id);

            if (obj != null)
            {
                obj.EndTimeUtc = endTime;
                obj.State = state;
                obj.CurrentProgressPercentage = 100;
                _context.SaveChanges();
            }
        }

        public bool UpdateTrigger(Guid id, string trigger)
        {
            var obj = _context.Jobs.Single(x => x.Id == id);

            if (obj != null)
            {
                obj.Trigger = trigger;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public void ResetAllJobs()
        {
            var jobs = _context.Jobs.ToList();
            jobs.ForEach(x =>
            {
                if (x.State == JobState.Running)
                {
                    x.State = JobState.Failed;
                    x.EndTimeUtc = DateTime.Now;
                }
            });

            _context.SaveChanges();
        }
    }
}
