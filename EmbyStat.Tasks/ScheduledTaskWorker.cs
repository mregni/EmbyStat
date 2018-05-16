using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Progress;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Enum;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using Serilog;

namespace EmbyStat.Tasks
{
    public class ScheduledTaskWorker : IScheduledTaskWorker
    {
        public event EventHandler<GenericEventArgs<double>> TaskProgress;
        public event EventHandler<GenericEventArgs<string>> TaskLogging;
        public IScheduledTask ScheduledTask { get; set; }
        private readonly ITaskManager _taskManager;
        private readonly ITaskRepository _taskRepository;

        public ScheduledTaskWorker(ITaskRepository taskRepository, IScheduledTask scheduledTask, ITaskManager taskManager)
        {
            _taskRepository = taskRepository;
            ScheduledTask = scheduledTask;
            _taskManager = taskManager;

            InitTriggerEvents();
        }

        private TaskResult _lastExecutionResult;
        private readonly object _lastExecutionResultSyncLock = new object();

        public TaskResult LastExecutionResult
        {
            get
            {
                lock (_lastExecutionResultSyncLock)
                {
                    if (_lastExecutionResult == null)
                    {
                        _lastExecutionResult = _taskRepository.GetTaskResultById(Id);
                    }
                }

                return _lastExecutionResult;
            }
            private set
            {
                _lastExecutionResult = value;

                lock (_lastExecutionResultSyncLock)
                {
                    _taskRepository.AddOrUpdateTaskResult(_lastExecutionResult);
                }
            }
        }

        public string Name => ScheduledTask.Name;
        public string Description => ScheduledTask.Description;
        public string Category => ScheduledTask.Category;

        private CancellationTokenSource CurrentCancellationTokenSource { get; set; }
        private DateTime CurrentExecutionStartTime { get; set; }

        public TaskState State
        {
            get
            {
                if (CurrentCancellationTokenSource != null)
                {
                    return CurrentCancellationTokenSource.IsCancellationRequested
                               ? TaskState.Cancelling
                               : TaskState.Running;
                }

                return TaskState.Idle;
            }
        }

        public double? CurrentProgress { get; private set; }
        private List<Tuple<TaskTriggerInfo, ITaskTrigger>> _triggers;
        private List<Tuple<TaskTriggerInfo, ITaskTrigger>> InternalTriggers
        {
            get => _triggers;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_triggers != null)
                {
                    DisposeTriggers();
                }

                _triggers = value.ToList();

                ReloadTriggerEvents(false);
            }
        }

        public List<TaskTriggerInfo> Triggers
        {
            get
            {
                var triggers = InternalTriggers;
                return triggers.Select(i => i.Item1).ToList();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var triggerList = value.Where(i => i != null).ToList();

                SaveTriggers(triggerList);

                InternalTriggers = triggerList.Select(i => new Tuple<TaskTriggerInfo, ITaskTrigger>(i, GetTrigger(i))).ToList();
            }
        }

        private string _id;
        public string Id => _id ?? (_id = GetMD5(ScheduledTask.GetType().FullName).ToString("N"));

        private void InitTriggerEvents()
        {
            _triggers = LoadTriggers();
            ReloadTriggerEvents(true);
        }

        public void ReloadTriggerEvents()
        {
            ReloadTriggerEvents(false);
        }

        private void ReloadTriggerEvents(bool isApplicationStartup)
        {
            foreach (var triggerInfo in InternalTriggers)
            {
                var trigger = triggerInfo.Item2;

                trigger.Stop();

                trigger.Triggered -= trigger_Triggered;
                trigger.Triggered += trigger_Triggered;
                trigger.Start(LastExecutionResult, Name, isApplicationStartup);
            }
        }

        async void trigger_Triggered(object sender, EventArgs e)
        {
            var trigger = (ITaskTrigger)sender;

            var configurableTask = ScheduledTask as IConfigurableScheduledTask;

            if (configurableTask != null && !configurableTask.IsEnabled)
            {
                return;
            }

            Log.Information("{0} fired for task: {1}", trigger.GetType().Name, Name);

            trigger.Stop();

            _taskManager.QueueScheduledTask(ScheduledTask, trigger.TaskOptions);

            await Task.Delay(1000).ConfigureAwait(false);

            trigger.Start(LastExecutionResult, Name, false);
        }

        private Task _currentTask;

        public async Task Execute(TaskOptions options)
        {
            var task = Task.Run(async () => await ExecuteInternal(options).ConfigureAwait(false));

            _currentTask = task;

            try
            {
                await task.ConfigureAwait(false);
            }
            finally
            {
                _currentTask = null;
                GC.Collect();
            }
        }

        private async Task ExecuteInternal(TaskOptions options)
        {
            if (CurrentCancellationTokenSource != null)
            {
                throw new InvalidOperationException("Cannot execute a Task that is already running");
            }

            var progress = new SimpleProgress<double>();
            var logProgress = new ProgressLogger();

            CurrentCancellationTokenSource = new CancellationTokenSource();

            Log.Information("Executing {0}", Name);

            ((TaskManager)_taskManager).OnTaskExecuting(this);

            progress.ProgressChanged += progress_ProgressChanged;
            logProgress.ProgressLogged += LogProgress_ProgressLogged;

            TaskCompletionStatus status;
            CurrentExecutionStartTime = DateTime.UtcNow;

            Exception failureException = null;

            try
            {
                if (options != null && options.MaxRuntimeTicks.HasValue)
                {
                    CurrentCancellationTokenSource.CancelAfter(TimeSpan.FromTicks(options.MaxRuntimeTicks.Value));
                }

                await ScheduledTask.Execute(CurrentCancellationTokenSource.Token, progress, logProgress).ConfigureAwait(false);

                status = TaskCompletionStatus.Completed;
            }
            catch (OperationCanceledException)
            {
                status = TaskCompletionStatus.Cancelled;
            }
            catch (Exception ex)
            {
                Log.Error("Error", ex);

                failureException = ex;

                status = TaskCompletionStatus.Failed;
            }

            var startTime = CurrentExecutionStartTime;
            var endTime = DateTime.UtcNow;

            progress.ProgressChanged -= progress_ProgressChanged;
            logProgress.ProgressLogged -= LogProgress_ProgressLogged;
            CurrentCancellationTokenSource.Dispose();
            CurrentCancellationTokenSource = null;
            CurrentProgress = null;

            OnTaskCompleted(startTime, endTime, status, failureException);
        }

        private void LogProgress_ProgressLogged(object sender, string e)
        {
            TaskLogging?.Invoke(this, new GenericEventArgs<string> { Argument = e });
        }

        void progress_ProgressChanged(object sender, double e)
        {
            e = Math.Min(e, 100);

            CurrentProgress = e;
            TaskProgress?.Invoke(this, new GenericEventArgs<double> { Argument = e });
        }

        public void Cancel()
        {
            if (State != TaskState.Running)
            {
                throw new InvalidOperationException("Cannot cancel a Task unless it is in the Running state.");
            }

            CancelIfRunning();
        }

        public void CancelIfRunning()
        {
            if (State == TaskState.Running)
            {
                Log.Information("Attempting to cancel Scheduled Task {0}", Name);
                CurrentCancellationTokenSource.Cancel();
            }
        }

        private List<Tuple<TaskTriggerInfo, ITaskTrigger>> LoadTriggers()
        {
            var settings = LoadTriggerSettings().Where(x => x.TaskKey == ScheduledTask.Key);

            return settings.Select(i => new Tuple<TaskTriggerInfo, ITaskTrigger>(i, GetTrigger(i))).ToList();
        }

        private List<TaskTriggerInfo> LoadTriggerSettings()
        {
            var triggers = _taskRepository.GetAllTaskTriggerInfo().ToList();
            if (triggers.Any(x => x.Type == ScheduledTask.Key))
            {
                return triggers;
            }

            return ScheduledTask.GetDefaultTriggers().ToList();

        }

        private void SaveTriggers(List<TaskTriggerInfo> triggers)
        {
            _taskRepository.SaveTaskInfoTriggers(triggers, ScheduledTask.Key);
        }

        private void OnTaskCompleted(DateTime startTime, DateTime endTime, TaskCompletionStatus status, Exception ex)
        {
            var elapsedTime = endTime - startTime;

            Log.Information("{0} {1} after {2} minute(s) and {3} seconds", Name, status, Math.Truncate(elapsedTime.TotalMinutes), elapsedTime.Seconds);

            var result = new TaskResult
            {
                StartTimeUtc = startTime,
                EndTimeUtc = endTime,
                Status = status,
                Name = Name,
                Id = Id,
                Key = ScheduledTask.Key
            };

            if (ex != null)
            {
                result.ErrorMessage = ex.Message;
                result.LongErrorMessage = ex.StackTrace;
            }

            LastExecutionResult = result;

            ((TaskManager)_taskManager).OnTaskCompleted(this, result);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                DisposeTriggers();

                var wassRunning = State == TaskState.Running;
                var startTime = CurrentExecutionStartTime;

                var token = CurrentCancellationTokenSource;
                if (token != null)
                {
                    try
                    {
                        Log.Information(Name + ": Cancelling");
                        token.Cancel();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error calling CancellationToken.Cancel();", ex);
                    }
                }
                var task = _currentTask;
                if (task != null)
                {
                    try
                    {
                        Log.Information(Name + ": Waiting on Task");
                        var exited = Task.WaitAll(new[] { task }, 2000);

                        if (exited)
                        {
                            Log.Information(Name + ": Task exited");
                        }
                        else
                        {
                            Log.Information(Name + ": Timed out waiting for task to stop");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error calling Task.WaitAll();", ex);
                    }
                }

                if (token != null)
                {
                    try
                    {
                        Log.Debug(Name + ": Disposing CancellationToken");
                        token.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error calling CancellationToken.Dispose();", ex);
                    }
                }
                if (wassRunning)
                {
                    OnTaskCompleted(startTime, DateTime.UtcNow, TaskCompletionStatus.Aborted, null);
                }
            }
        }

        private ITaskTrigger GetTrigger(TaskTriggerInfo info)
        {
            var options = new TaskOptions
            {
                MaxRuntimeTicks = info.MaxRuntimeTicks
            };

            if (info.Type.Equals(typeof(DailyTrigger).Name, StringComparison.OrdinalIgnoreCase))
            {
                if (!info.TimeOfDayTicks.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return new DailyTrigger
                {
                    TimeOfDay = TimeSpan.FromTicks(info.TimeOfDayTicks.Value),
                    TaskOptions = options
                };
            }

            if (info.Type.Equals(typeof(IntervalTrigger).Name, StringComparison.OrdinalIgnoreCase))
            {
                if (!info.IntervalTicks.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return new IntervalTrigger
                {
                    Interval = TimeSpan.FromTicks(info.IntervalTicks.Value),
                    TaskOptions = options
                };
            }

            if (info.Type.Equals(typeof(StartupTrigger).Name, StringComparison.OrdinalIgnoreCase))
            {
                return new StartupTrigger();
            }

            throw new ArgumentException("Unrecognized trigger type: " + info.Type);
        }

        private void DisposeTriggers()
        {
            foreach (var triggerInfo in InternalTriggers)
            {
                var trigger = triggerInfo.Item2;
                trigger.Triggered -= trigger_Triggered;
                trigger.Stop();
            }
        }

        private Guid GetMD5(string str)
        {
            using (var provider = MD5.Create())
            {
                return new Guid(provider.ComputeHash(Encoding.Unicode.GetBytes(str)));
            }
           
        }
    }
}
