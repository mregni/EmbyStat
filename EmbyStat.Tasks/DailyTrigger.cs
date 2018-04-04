using System;
using System.Globalization;
using System.Threading;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
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

            Log.Information("Daily trigger for {0} set to fire at {1}, which is {2} minutes from now.", taskName, triggerDate.ToString(), dueTime.TotalMinutes.ToString(CultureInfo.InvariantCulture));

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
