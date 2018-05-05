using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Services.Converters
{
    public static class TaskHelpers
    {
        public static TaskInfo ConvertToTaskInfo(IScheduledTaskWorker task)
        {
            return new TaskInfo
            {
                Name = task.Name,
                CurrentProgressPercentage = task.CurrentProgress,
                State = task.State,
                Id = task.Id,
                LastExecutionResult = task.LastExecutionResult,
                Triggers = task.Triggers,
                Description = task.Description,
                Category = task.Category
            };
        }
    }
}
