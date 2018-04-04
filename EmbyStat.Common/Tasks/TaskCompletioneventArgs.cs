using System;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Common.Tasks
{
    public class TaskCompletionEventArgs : EventArgs
    {
        public IScheduledTaskWorker Task { get; set; }
        public TaskResult Result { get; set; }
    }
}
