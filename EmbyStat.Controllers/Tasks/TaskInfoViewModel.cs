using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Enum;

namespace EmbyStat.Controllers.Tasks
{
    public class TaskInfoViewModel
    {
        public string Name { get; set; }
        public int State { get; set; }
        public double? CurrentProgressPercentage { get; set; }
        public string Id { get; set; }
        public TaskResultViewModel LastExecutionResult { get; set; }
        public List<TaskTriggerInfoViewModel> Triggers { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
