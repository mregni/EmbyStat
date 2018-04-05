using System.Collections.Generic;
using EmbyStat.Common.Tasks;

namespace EmbyStat.Services.Tasks
{
    public interface ITaskService
    {
        List<TaskInfo> GetAllTasks();
        List<TaskStatus> GetStates();
        TaskStatus GetStateByTaskId(string id);
    }
}
