using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Enum;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.EmbyTask;
using Serilog;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Class TaskManager
    /// </summary>
    public class TaskManager : ITaskManager
    {
        public string BoeName { get; set; }
        public event EventHandler<GenericEventArgs<IScheduledTaskWorker>> TaskExecuting;
        public event EventHandler<TaskCompletionEventArgs> TaskCompleted;

        public List<IScheduledTaskWorker> ScheduledTasks { get; private set; }

        private readonly ConcurrentQueue<Tuple<Type, TaskOptions>> _taskQueue = new ConcurrentQueue<Tuple<Type, TaskOptions>>();
        private readonly ITaskRepository _taskRepository;

        public TaskManager(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;

            ScheduledTasks = new List<IScheduledTaskWorker>();
        }

        public void CancelIfRunningAndQueue<T>(TaskOptions options) where T : IScheduledTask
        {
            var task = ScheduledTasks.First(t => t.ScheduledTask.GetType() == typeof(T));
            ((ScheduledTaskWorker)task).CancelIfRunning();

            QueueScheduledTask<T>(options);
        }

        public void CancelIfRunningAndQueue<T>()  where T : IScheduledTask
        {
            CancelIfRunningAndQueue<T>(new TaskOptions());
        }

        public void CancelIfRunning<T>()
                 where T : IScheduledTask
        {
            var task = ScheduledTasks.First(t => t.ScheduledTask.GetType() == typeof(T));
            ((ScheduledTaskWorker)task).CancelIfRunning();
        }

        public void QueueScheduledTask<T>(TaskOptions options) where T : IScheduledTask
        {
            var scheduledTask = ScheduledTasks.FirstOrDefault(t => t.ScheduledTask.GetType() == typeof(T));

            if (scheduledTask == null)
            {
                Log.Error("Unable to find scheduled task of type {0} in QueueScheduledTask.", typeof(T).Name);
            }
            else
            {
                QueueScheduledTask(scheduledTask, options);
            }
        }

        public void QueueScheduledTask<T>() where T : IScheduledTask
        {
            QueueScheduledTask<T>(new TaskOptions());
        }

        public void QueueIfNotRunning<T>() where T : IScheduledTask
        {
            var task = ScheduledTasks.First(t => t.ScheduledTask.GetType() == typeof(T));

            if (task.State != TaskState.Running)
            {
                QueueScheduledTask<T>(new TaskOptions());
            }
        }

        public void Execute<T>() where T : IScheduledTask
        {
            var scheduledTask = ScheduledTasks.FirstOrDefault(t => t.ScheduledTask.GetType() == typeof(T));

            if (scheduledTask == null)
            {
                Log.Error("Unable to find scheduled task of type {0} in Execute.", typeof(T).Name);
            }
            else
            {
                var type = scheduledTask.ScheduledTask.GetType();

                Log.Information("Queueing task {0}", type.Name);

                lock (_taskQueue)
                {
                    if (scheduledTask.State == TaskState.Idle)
                    {
                        Execute(scheduledTask, new TaskOptions());
                    }
                }
            }
        }

        public void QueueScheduledTask(IScheduledTask task, TaskOptions options)
        {
            var scheduledTask = ScheduledTasks.FirstOrDefault(t => t.ScheduledTask.GetType() == task.GetType());

            if (scheduledTask == null)
            {
                Log.Error("Unable to find scheduled task of type {0} in QueueScheduledTask.", task.GetType().Name);
            }
            else
            {
                QueueScheduledTask(scheduledTask, options);
            }
        }

        private void QueueScheduledTask(IScheduledTaskWorker task, TaskOptions options)
        {
            var type = task.ScheduledTask.GetType();

            Log.Information("Queueing task {0}", type.Name);

            lock (_taskQueue)
            {
                if (task.State == TaskState.Idle)
                {
                    Execute(task, options);
                    return;
                }

                _taskQueue.Enqueue(new Tuple<Type, TaskOptions>(type, options));
            }
        }

        public void AddTasks(IEnumerable<IScheduledTask> tasks)
        {
            var myTasks = ScheduledTasks.ToList();

            var list = tasks.ToList();
            myTasks.AddRange(list.Select(t => new ScheduledTaskWorker(_taskRepository, t, this)));

            ScheduledTasks = myTasks.ToList();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            foreach (var task in ScheduledTasks)
            {
                task.Dispose();
            }
        }

        public void Cancel(IScheduledTaskWorker task)
        {
            ((ScheduledTaskWorker)task).Cancel();
        }

        public Task Execute(IScheduledTaskWorker task, TaskOptions options)
        {
            return ((ScheduledTaskWorker)task).Execute(options);
        }

        internal void OnTaskExecuting(IScheduledTaskWorker task)
        {
            TaskExecuting?.Invoke(this, new GenericEventArgs<IScheduledTaskWorker> { Argument = task });
        }

        internal void OnTaskCompleted(IScheduledTaskWorker task, TaskResult result)
        {
            TaskCompleted?.Invoke(this, new TaskCompletionEventArgs()
            {
                Result = result,
                Task = task

            });

            ExecuteQueuedTasks();
        }

        private void ExecuteQueuedTasks()
        {
            Log.Information("ExecuteQueuedTasks");

            lock (_taskQueue)
            {
                var list = new List<Tuple<Type, TaskOptions>>();

                Tuple<Type, TaskOptions> item;
                while (_taskQueue.TryDequeue(out item))
                {
                    if (list.All(i => i.Item1 != item.Item1))
                    {
                        list.Add(item);
                    }
                }

                foreach (var enqueuedType in list)
                {
                    var scheduledTask = ScheduledTasks.First(t => t.ScheduledTask.GetType() == enqueuedType.Item1);

                    if (scheduledTask.State == TaskState.Idle)
                    {
                        Execute(scheduledTask, enqueuedType.Item2);
                    }
                }
            }
        }
    }
}
