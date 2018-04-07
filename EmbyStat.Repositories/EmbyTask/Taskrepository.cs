using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public List<TaskTriggerInfo> GetAllTaskTriggerInfo()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TaskTriggerInfos.ToList();
            }
        }

        public void SaveTaskInfoTriggers(List<TaskTriggerInfo> list, string key)
        {
            using (var context = new ApplicationDbContext())
            {
                context.TaskTriggerInfos.RemoveRange(context.TaskTriggerInfos.Where(x => x.TaskKey == key));
                context.SaveChanges();
                context.TaskTriggerInfos.AddRange(list);
                context.SaveChanges();
            }
        }

        public void AddOrUpdateTaskResult(TaskResult lastExecutionResult)
        {
            using (var context = new ApplicationDbContext())
            {
                var result = context.TaskResults.AsNoTracking().SingleOrDefault(x => x.Id == lastExecutionResult.Id);
                if (result == null)
                {
                    context.TaskResults.Add(lastExecutionResult);
                }
                else
                {
                    context.Entry(lastExecutionResult).State = EntityState.Modified;
                }
                
                context.SaveChanges();
            }
        }
    }
}
