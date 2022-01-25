using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.BackgroundTasks.Interfaces;
using Microsoft.Extensions.Hosting;
using NLog;

namespace EmbyStat.BackgroundTasks
{
    public class Monitor
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger _logger;
        private readonly IEnumerable<IBackgroundTask> _tasks;
        private readonly CancellationToken _cancellationToken;

        public Monitor(IBackgroundTaskQueue taskQueue, IHostApplicationLifetime applicationLifetime, IEnumerable<IBackgroundTask> tasks)
        {
            _taskQueue = taskQueue;
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _tasks = tasks;
        }

        public Task StartMonitorLoop()
        {
            return Task.Run(MonitorAsync, _cancellationToken);
        }

        private void MonitorAsync()
        {
            foreach (var task in _tasks)
            {
                async void callBack(object state) => await QueueTask(task);
                var timer = new Timer(callBack, task.GetAutoEvent(), task.GetTrigger(), task.GetTrigger());
            }
        }

        private async Task QueueTask(IBackgroundTask task)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(task.RunJob);
        }
    }

    class TimerExample
    {
        static void Main()
        {
            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.
            var autoEvent = new AutoResetEvent(false);

            var statusChecker = new StatusChecker(10);

            // Create a timer that invokes CheckStatus after one second, 
            // and every 1/4 second thereafter.
            Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                DateTime.Now);
            var stateTimer = new Timer(statusChecker.CheckStatus,
                autoEvent, 1000, 250);

            // When autoEvent signals, change the period to every half second.
            autoEvent.WaitOne();
            stateTimer.Change(0, 500);
            Console.WriteLine("\nChanging period to .5 seconds.\n");

            // When autoEvent signals the second time, dispose of the timer.
            autoEvent.WaitOne();
            stateTimer.Dispose();
            Console.WriteLine("\nDestroying timer.");
        }
    }

    class StatusChecker
    {
        private int invokeCount;
        private int maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            Console.WriteLine("{0} Checking status {1,2}.",
                DateTime.Now.ToString("h:mm:ss.fff"),
                (++invokeCount).ToString());

            if (invokeCount == maxCount)
            {
                // Reset the counter and signal the waiting thread.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
    }
}
