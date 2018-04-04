using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Tasks.Tasks
{
    public class PingEmbyTask : IScheduledTask
    {
        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                progress.Report(20);
                Thread.Sleep(10000);
                progress.Report(40);
                Thread.Sleep(10000);
                progress.Report(60);
                Thread.Sleep(10000);
                progress.Report(80);
                Thread.Sleep(10000);
                progress.Report(100);
            }, cancellationToken);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>();
        }

        public string Name => "Check Emby connection";
        public string Key => "PingEmbyServer";
        public string Description => "Ping the Emby server to check if he is still online";
        public string Category => "System";
    }
}
