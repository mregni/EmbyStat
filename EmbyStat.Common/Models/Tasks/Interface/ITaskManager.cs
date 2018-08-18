using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmbyStat.Common.Models.Tasks.Interface
{
    public interface ITaskManager
    {
        List<IScheduledTaskWorker> ScheduledTasks { get; }
        event EventHandler<GenericEventArgs<IScheduledTaskWorker>> TaskExecuting;
        event EventHandler<TaskCompletionEventArgs> TaskCompleted;
        void AddTasks(IEnumerable<IScheduledTask> tasks);
        void Cancel(IScheduledTaskWorker task);
        void CancelIfRunning<T>() where T : IScheduledTask;
        void CancelIfRunningAndQueue<T>(TaskOptions options) where T : IScheduledTask;
        void CancelIfRunningAndQueue<T>() where T : IScheduledTask;
        Task Execute(IScheduledTaskWorker task, TaskOptions options);
        void Execute<T>() where T : IScheduledTask;
        void QueueIfNotRunning<T>() where T : IScheduledTask;
        void QueueScheduledTask<T>(TaskOptions options) where T : IScheduledTask;
        void QueueScheduledTask<T>() where T : IScheduledTask;
        void QueueScheduledTask(IScheduledTask task, TaskOptions options);
    }
}
