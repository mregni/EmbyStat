using System.Collections.Generic;
using EmbyStat.Common.Tasks;

namespace EmbyStat.Repositories.EmbyTask
{
    public interface ITaskRepository
    {
        TaskResult GetTaskResultById(string id);
        void SaveTaskResult(TaskResult result);
        List<TaskTriggerInfo> GetAllTaskTriggerInfo();
        void SaveTaskInfoTriggers(List<TaskTriggerInfo> list);
    }
}
