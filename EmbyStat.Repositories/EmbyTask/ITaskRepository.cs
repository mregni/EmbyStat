using System.Collections.Generic;
using EmbyStat.Common.Tasks;

namespace EmbyStat.Repositories.EmbyTask
{
    public interface ITaskRepository
    {
        TaskResult GetTaskResultById(string id);
        List<TaskTriggerInfo> GetAllTaskTriggerInfo();
        void SaveTaskInfoTriggers(List<TaskTriggerInfo> list);
        void AddOrUpdateTaskResult(TaskResult lastExecutionResult);
    }
}
