using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Tasks;
using Serilog;

namespace EmbyStat.Tasks.Interfaces
{
    public interface ITaskTrigger
    {
        TaskOptions TaskOptions { get; set; }

        event EventHandler<EventArgs> Triggered;

        void Start(TaskResult lastResult, string taskName, bool isApplicationStartup);
        void Stop();
    }
}
