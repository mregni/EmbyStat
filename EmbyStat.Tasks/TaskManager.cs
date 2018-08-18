using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Common.Models.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Class TaskManager
    /// </summary>
    public class TaskManager : ITaskManager
    {
        public event EventHandler<GenericEventArgs<IScheduledTaskWorker>> TaskExecuting;
        public event EventHandler<TaskCompletionEventArgs> TaskCompleted;

        public List<IScheduledTaskWorker> ScheduledTasks { get; private set; }

        private readonly ConcurrentQueue<Tuple<Type, TaskOptions>> _taskQueue = new ConcurrentQueue<Tuple<Type, TaskOptions>>();
        private readonly ITaskRepository _taskRepository;
        private readonly IHubContext<TaskHub> _taskHubContext;

        public TaskManager(ITaskRepository taskRepository, IHubContext<TaskHub> taskHubContext)
        {
            _taskRepository = taskRepository;
            _taskHubContext = taskHubContext;

            ScheduledTasks = new List<IScheduledTaskWorker>();

            TaskExecuting += TaskManager_TaskExecuting;
            TaskCompleted += TaskManager_TaskCompleted;
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
                Log.Error($"{Constants.LogPrefix.TaskManager}\tUnable to find scheduled task of type {typeof(T).Name} in QueueScheduledTask.");
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
                Log.Error($"{Constants.LogPrefix.TaskManager}\tUnable to find scheduled task of type {typeof(T).Name} in Execute.");
            }
            else
            {
                var type = scheduledTask.ScheduledTask.GetType();

                Log.Information($"{Constants.LogPrefix.TaskManager}\tQueueing task {type.Name}");

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
                Log.Error($"{Constants.LogPrefix.TaskManager}\tUnable to find scheduled task of type {task.GetType().Name} in QueueScheduledTask.");
            }
            else
            {
                QueueScheduledTask(scheduledTask, options);
            }
        }

        private void QueueScheduledTask(IScheduledTaskWorker task, TaskOptions options)
        {
            var type = task.ScheduledTask.GetType();

            Log.Information($"{Constants.LogPrefix.TaskManager}\tQueueing task {type.Name}");

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

        public void UpdateTriggers(TaskInfo taskInfo)
        {
            var task = ScheduledTasks.SingleOrDefault(x => x.Id == taskInfo.Id);
            if (task != null)
            {
                task.Triggers = taskInfo.Triggers;
            }
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
            Log.Information($"{Constants.LogPrefix.TaskManager}\tExecuteQueuedTasks");

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

        private async void TaskManager_TaskCompleted(object sender, TaskCompletionEventArgs e)
        {
            e.Task.TaskProgress -= Argument_TaskProgress;
            e.Task.TaskLogging -= Argument_TaskLogging;
            await _taskHubContext.Clients.All.SendAsync("ReceiveInfo", GetSendData());
        }

        private async void TaskManager_TaskExecuting(object sender, GenericEventArgs<IScheduledTaskWorker> e)
        {
            e.Argument.TaskProgress += Argument_TaskProgress;
            e.Argument.TaskLogging += Argument_TaskLogging;
            await _taskHubContext.Clients.All.SendAsync("ReceiveInfo", GetSendData());
        }

        private async void Argument_TaskLogging(object sender, GenericEventArgs<ProgressLog> e)
        {
            await _taskHubContext.Clients.All.SendAsync("ReceiveLog", e.Argument);
        }

        private async void Argument_TaskProgress(object sender, GenericEventArgs<double> e)
        {
            await _taskHubContext.Clients.All.SendAsync("ReceiveInfo", GetSendData());
        }

        private IEnumerable<TaskInfo> GetSendData()
        {
            return ScheduledTasks.OrderBy(x => x.Name).Select(TaskHelpers.ConvertToTaskInfo);
        }
    }
}
