using System;
using System.Threading;
using EmbyStat.Common;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Interface;
using Serilog;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Represents a task trigger that fires everyday
    /// </summary>
    public class DailyTrigger : ITaskTrigger
    {
        public TimeSpan TimeOfDay { get; set; }
        public TaskOptions TaskOptions { get; set; }
        public event EventHandler<EventArgs> Triggered;
        private Timer Timer { get; set; }

        public void Start(TaskResult lastResult, string taskName, bool isApplicationStartup)
        {
            DisposeTimer();

            var now = DateTime.Now;

            var triggerDate = now.TimeOfDay > TimeOfDay ? now.Date.AddDays(1) : now.Date;
            triggerDate = triggerDate.Add(TimeOfDay);

            var dueTime = triggerDate - now;

            Log.Information($"{Constants.LogPrefix.TaskWorker}\tDaily trigger for {taskName} set to fire at {triggerDate}, which is {Math.Floor(dueTime.TotalMinutes)} minutes from now.");
            Timer = new Timer(state => OnTriggered(), null, dueTime, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            Timer?.Dispose();
        }

        private void OnTriggered()
        {
            Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
