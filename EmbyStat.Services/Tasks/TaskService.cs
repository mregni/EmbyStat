using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Helpers;
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
            return _taskManager.ScheduledTasks.OrderBy(x => x.Name).Select(TaskHelpers.ConvertToTaskInfo).ToList();

        }

        public List<TaskStatus> GetStates()
        {
            return _taskManager.ScheduledTasks.Select(x => new TaskStatus {Id = x.Id, State = x.State, CurrentProgress = x.CurrentProgress}).ToList();
        }

        public TaskStatus GetStateByTaskId(string id)
        {
            return _taskManager.ScheduledTasks.Where(x => x.Id == id).Select(x => new TaskStatus { Id = x.Id, State = x.State, CurrentProgress = x.CurrentProgress }).SingleOrDefault();
        }
    }
}
