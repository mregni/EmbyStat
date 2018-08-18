using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Tasks.Interface
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
        event EventHandler<GenericEventArgs<ProgressLog>> TaskLogging;
        void ReloadTriggerEvents();
    }
}
