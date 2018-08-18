using System;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Tasks
{
    public class TaskResult
    {
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public TaskCompletionStatus Status { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
        public string LongErrorMessage { get; set; }
    }
}
