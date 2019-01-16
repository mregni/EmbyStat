using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Entities
{
    public class Job
    {
        [Key]
        public Guid Id { get; set; }
        public JobState State { get; set; }
        public double? CurrentProgressPercentage { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Trigger { get; set; }
    }
}
