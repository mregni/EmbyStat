using System.Collections.Generic;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Tasks
{
    public class TaskInfo
    {
        public string Name { get; set; }
        public TaskState State { get; set; }
        public double? CurrentProgressPercentage { get; set; }
        public string Id { get; set; }
        public TaskResult LastExecutionResult { get; set; }
        public List<TaskTriggerInfo> Triggers { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
