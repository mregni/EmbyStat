using System;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Tasks
{
    public class JobLog
    {
        public string JobName { get; set; }
        public string Value { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public ProgressLogType Type { get; set; }
    }
}
