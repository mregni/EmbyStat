using System.Collections.Generic;
using EmbyStat.Common.Tasks;

namespace EmbyStat.Services.Interfaces
{
    public interface ITaskService
    {
        List<TaskInfo> GetAllTasks();
        void UpdateTriggers(TaskInfo task);
        void FireTask(string id);
    }
}
