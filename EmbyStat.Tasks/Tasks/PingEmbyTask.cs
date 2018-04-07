using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using Microsoft.AspNetCore.Builder;

namespace EmbyStat.Tasks.Tasks
{
    public class PingEmbyTask : IScheduledTask
    {
        private readonly IApplicationBuilder _app;

        public PingEmbyTask(IApplicationBuilder app)
        {
            _app = app;
        }

        public string Name => "Check Emby connection";
        public string Key => "PingEmbyServer";
        public string Description => "TASKS.PINGEMBYSERVERDESCRIPTION";
        public string Category => "Emby";

        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                progress.Report(10);
                Thread.Sleep(10000);
                progress.Report(20);
                Thread.Sleep(10000);
                progress.Report(40);
                Thread.Sleep(10000);
                progress.Report(60);
                Thread.Sleep(10000);
                progress.Report(75);
                Thread.Sleep(1000);
                progress.Report(80);
                Thread.Sleep(1000);
                progress.Report(85);
                Thread.Sleep(1000);
                progress.Report(90);
                progress.Report(100);
            }, cancellationToken);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ Type = "IntervalTrigger", IntervalTicks = 1200000000, TaskKey = Key}
            };
        }
    }
}
