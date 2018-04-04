using System.Collections.Generic;
using MediaBrowser.Model.Tasks;

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
