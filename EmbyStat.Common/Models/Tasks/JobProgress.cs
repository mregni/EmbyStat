﻿using System;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Tasks
{
    public class JobProgress
    {
        public Guid Id { get; set; }
        public JobState State { get; set; }
        public double? CurrentProgressPercentage { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
    }
}
