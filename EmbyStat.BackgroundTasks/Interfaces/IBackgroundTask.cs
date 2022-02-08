using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.BackgroundTasks.Interfaces
{
    public interface IBackgroundTask
    {
        public TimeSpan GetTrigger();
        public Task RunJob(CancellationToken token);
        AutoResetEvent GetAutoEvent();
    }
}
