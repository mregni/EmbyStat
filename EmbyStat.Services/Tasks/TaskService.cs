using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly ITaskManager _taskManager;
        public TaskService(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }
        public List<TaskInfo> GetAllTasks()
        {
            return _taskManager.ScheduledTasks.OrderBy(x => x.Name).Select(ConvertToTaskInfo).ToList();

        }

        private TaskInfo ConvertToTaskInfo(IScheduledTaskWorker task)
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
