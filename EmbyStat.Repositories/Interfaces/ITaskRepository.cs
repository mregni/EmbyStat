using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        TaskResult GetTaskResultById(string id);
        void AddOrUpdateTaskResult(TaskResult lastExecutionResult);
        TaskResult GetLatestTaskByKeyAndStatus(string key, TaskCompletionStatus status);
    }
}
