using System.Collections.Generic;

namespace EmbyStat.Controllers.ViewModels.Task
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
