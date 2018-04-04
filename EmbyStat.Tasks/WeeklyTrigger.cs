using System;
using System.Threading;
using MediaBrowser.Model.Tasks;
using Serilog;
using ITaskTrigger = EmbyStat.Tasks.Interfaces.ITaskTrigger;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Represents a task trigger that fires on a weekly basis
    /// </summary>
    public class WeeklyTrigger : ITaskTrigger
    {
        public TimeSpan TimeOfDay { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TaskOptions TaskOptions { get; set; }
        public event EventHandler<EventArgs> Triggered;
        private Timer Timer { get; set; }
        public void Start(TaskResult lastResult, string taskName, bool isApplicationStartup)
        {
            DisposeTimer();

            var triggerDate = GetNextTriggerDateTime();

            Timer = new Timer(state => OnTriggered(), null, triggerDate - DateTime.Now, TimeSpan.FromMilliseconds(-1));
        }

        private DateTime GetNextTriggerDateTime()
        {
            var now = DateTime.Now;

            if (now.DayOfWeek == DayOfWeek)
            {
                return now.TimeOfDay < TimeOfDay ? now.Date.Add(TimeOfDay) : now.Date.AddDays(7).Add(TimeOfDay);
            }

            var triggerDate = now.Date;

            while (triggerDate.DayOfWeek != DayOfWeek)
            {
                triggerDate = triggerDate.AddDays(1);
            }

            return triggerDate.Add(TimeOfDay);
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
