using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.Job;

namespace Tests.Unit.Controllers.Builders
{
    public class JobViewModelBuilder
    {
        private readonly JobViewModel _model;

        public JobViewModelBuilder(Job job)
        {
            _model = new JobViewModel
            {
                CurrentProgressPercentage = job.CurrentProgressPercentage,
                Description = job.Description,
                EndTimeUtc = job.EndTimeUtc,
                Id = job.Id,
                StartTimeUtc = job.StartTimeUtc,
                State = (int)job.State,
                Title = job.Title,
                Trigger = job.Trigger
            };
        }

        public JobViewModel Build()
        {
            return _model;
        }
    }
}
