using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Services.Interfaces;
using Serilog;
using TaskHelpers = EmbyStat.Common.Converters.TaskHelpers;

namespace EmbyStat.Services
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
            return _taskManager.ScheduledTasks.OrderBy(x => x.Name).Select(TaskHelpers.ConvertToTaskInfo).ToList();
        }

        public void UpdateTriggers(TaskInfo taskInfo)
        {
            var task = _taskManager.ScheduledTasks.SingleOrDefault(x => x.Id == taskInfo.Id);

            if (task == null)
            {
                Log.Error($"No task was found for id {taskInfo.Id}");
                throw new BusinessException("INTERNAL_ERROR");
            }

            taskInfo.Triggers.ForEach(x => x.TaskKey = task.ScheduledTask.Key);

            task.Triggers = taskInfo.Triggers;
        }

        public void FireTask(string id)
        {
            var task = _taskManager.ScheduledTasks.SingleOrDefault(x => x.Id == id);
            if (task == null)
            {
                Log.Error($"No task was found for id {id}");
                throw new BusinessException("INTERNAL_ERROR");
            }

            _taskManager.Execute(task, new TaskOptions());
        }
    }
}
