using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Tasks;
using IScheduledTaskWorker = EmbyStat.Tasks.Interfaces.IScheduledTaskWorker;

namespace EmbyStat.Tasks
{
    public class TaskCompletionEventArgs : EventArgs
    {
        public IScheduledTaskWorker Task { get; set; }
        public TaskResult Result { get; set; }
    }
}
