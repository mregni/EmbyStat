using System;
using EmbyStat.Common.Models.Tasks.Interface;

namespace EmbyStat.Common.Models.Tasks
{
    public class TaskCompletionEventArgs : EventArgs
    {
        public IScheduledTaskWorker Task { get; set; }
        public TaskResult Result { get; set; }
    }
}
