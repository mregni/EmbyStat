using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Tasks;
using TaskTriggerInfo = EmbyStat.Repositories.EmbyTask.TaskTriggerInfo;

namespace EmbyStat.Tasks.Interfaces
{
    public interface IScheduledTaskWorker : IDisposable
    {
        IScheduledTask ScheduledTask { get; }
        TaskResult LastExecutionResult { get; }
        string Name { get; }
        string Description { get; }
        string Category { get; }
        TaskState State { get; }
        double? CurrentProgress { get; }
        TaskTriggerInfo[] Triggers { get; set; }
        string Id { get; }
        event EventHandler<GenericEventArgs<double>> TaskProgress;
        void ReloadTriggerEvents();
    }
}
