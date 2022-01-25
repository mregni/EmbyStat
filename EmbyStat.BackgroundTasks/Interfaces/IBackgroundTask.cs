using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
