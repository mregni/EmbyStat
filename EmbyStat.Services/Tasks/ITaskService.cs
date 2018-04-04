using System;
using System.Collections.Generic;
using System.Text;
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
