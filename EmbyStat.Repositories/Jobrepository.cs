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
        public IEnumerable<Job> GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Jobs.ToList();
            }
        }

        public Job GetById(Guid id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Jobs.Single(x => x.Id == id);
            }
        }

        public void StartJob(Job job)
        {
            using (var context = new ApplicationDbContext())
            {
                var obj = context.Jobs.Single(x => x.Id == job.Id);

                if (obj != null)
                {
                    obj.State = job.State;
                    obj.StartTimeUtc = job.StartTimeUtc;
                    obj.EndTimeUtc = job.EndTimeUtc;
                    obj.CurrentProgressPercentage = job.CurrentProgressPercentage;
                    context.SaveChanges();
                }
            }
        }

        public void EndJob(Guid id, DateTime endTime, JobState state)
        {
            using (var context = new ApplicationDbContext())
            {
                var obj = context.Jobs.Single(x => x.Id == id);

                if (obj != null)
                {
                    obj.EndTimeUtc = endTime;
                    obj.State = state;
                    obj.CurrentProgressPercentage = 100;
                    context.SaveChanges();
                }
            }
        }

        public bool UpdateTrigger(Guid id, string trigger)
        {
            using (var context = new ApplicationDbContext())
            {
                var obj = context.Jobs.Single(x => x.Id == id);

                if (obj != null)
                {
                    obj.Trigger = trigger;
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
    }
}
