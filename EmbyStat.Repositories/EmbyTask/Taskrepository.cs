using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Tasks;

namespace EmbyStat.Repositories.EmbyTask
{
    public class TaskRepository : ITaskRepository
    {
        public TaskResult GetTaskResultById(string id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TaskResults.SingleOrDefault(x => x.Id == id);
            }
        }

        public void SaveTaskResult(TaskResult result)
        {
            using (var context = new ApplicationDbContext())
            {
                context.TaskResults.Add(result);
                context.SaveChanges();
            }
        }

        public List<TaskTriggerInfo> GetAllTaskTriggerInfo()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TaskTriggerInfos.ToList();
            }
        }

        public void SaveTaskInfoTriggers(List<TaskTriggerInfo> list)
        {
            using (var context = new ApplicationDbContext())
            {
                context.TaskTriggerInfos.AddRange(list);
                context.SaveChanges();
            }
        }
    }
}
