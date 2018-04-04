using System;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;
using Serilog;
using ITaskTrigger = EmbyStat.Tasks.Interfaces.ITaskTrigger;

namespace EmbyStat.Tasks
{
    /// <summary>
    /// Class StartupTaskTrigger
    /// </summary>
    public class StartupTrigger : ITaskTrigger
    {
        public event EventHandler<EventArgs> Triggered;
        public int DelayMs { get; set; }
        public TaskOptions TaskOptions { get; set; }

        public StartupTrigger()
        {
            DelayMs = 3000;
        }

        public async void Start(TaskResult lastResult, string taskName, bool isApplicationStartup)
        {
            if (isApplicationStartup)
            {
                await Task.Delay(DelayMs).ConfigureAwait(false);

                OnTriggered();
            }
        }

        public void Stop()
        {
        }

        private void OnTriggered()
        {
            Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
