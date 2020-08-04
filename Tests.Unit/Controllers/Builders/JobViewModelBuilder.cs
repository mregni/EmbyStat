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
                EndTimeUtcIso = job.EndTimeUtc?.ToString("O") ?? string.Empty,
                Id = job.Id,
                StartTimeUtcIso = job.StartTimeUtc?.ToString("O") ?? string.Empty,
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
