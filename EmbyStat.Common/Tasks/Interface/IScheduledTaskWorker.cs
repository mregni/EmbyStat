using System;
using System.Collections.Generic;
using EmbyStat.Common.Tasks.Enum;

namespace EmbyStat.Common.Tasks.Interface
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
        List<TaskTriggerInfo> Triggers { get; set; }
        string Id { get; }
        event EventHandler<GenericEventArgs<double>> TaskProgress;
        void ReloadTriggerEvents();
    }
}
