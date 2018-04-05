using System;
using System.Linq;
using System.Threading;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Represents a task trigger that runs repeatedly on an interval
    /// </summary>
    public class IntervalTrigger : ITaskTrigger
    {
        public TimeSpan Interval { get; set; }
        public TaskOptions TaskOptions { get; set; }
        public event EventHandler<EventArgs> Triggered;
        private Timer Timer { get; set; }
        private DateTime _lastStartDate;
        public void Start(TaskResult lastResult, string taskName, bool isApplicationStartup)
        {
            DisposeTimer();

            DateTime triggerDate;

            if (lastResult == null)
            {
                triggerDate = DateTime.UtcNow.AddHours(1);
            }
            else
            {
                triggerDate = new[] { lastResult.EndTimeUtc, _lastStartDate }.Max().Add(Interval);
            }

            if (DateTime.UtcNow > triggerDate)
            {
                triggerDate = DateTime.UtcNow.AddMinutes(1);
            }

            var dueTime = triggerDate - DateTime.UtcNow;
            var maxDueTime = TimeSpan.FromDays(7);

            if (dueTime > maxDueTime)
            {
                dueTime = maxDueTime;
            }

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
            DisposeTimer();

            if (Triggered != null)
            {
                _lastStartDate = DateTime.UtcNow;
                Triggered(this, EventArgs.Empty);
            }
        }
    }
}
