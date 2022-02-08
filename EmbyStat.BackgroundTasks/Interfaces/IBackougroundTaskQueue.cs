using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.BackgroundTasks.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);
        ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
