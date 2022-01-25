using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.BackgroundTasks.Interfaces;
using Microsoft.Extensions.Hosting;

namespace EmbyStat.BackgroundTasks
{
    public class QueuedHostedService : BackgroundService
    {
        public QueuedHostedService(IBackgroundTaskQueue taskQueue)
        {
            TaskQueue = taskQueue;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    Console.WriteLine("Starting {0}", nameof(task));
                    await task(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred executing {0}.", nameof(task));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
