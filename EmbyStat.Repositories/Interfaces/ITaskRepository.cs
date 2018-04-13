using System.Collections.Generic;
using EmbyStat.Common.Tasks;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        TaskResult GetTaskResultById(string id);
        List<TaskTriggerInfo> GetAllTaskTriggerInfo();
        void SaveTaskInfoTriggers(List<TaskTriggerInfo> list, string key);
        void AddOrUpdateTaskResult(TaskResult lastExecutionResult);
    }
}
