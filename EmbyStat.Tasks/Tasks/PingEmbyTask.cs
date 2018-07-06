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
        public PingEmbyTask(IApplicationBuilder app)
        {
           
        }

        public string Name => "TASKS.PINGEMBYSERVERTITLE";
        public string Key => "PingEmbyServer";
        public string Description => "TASKS.PINGEMBYSERVERDESCRIPTION";
        public string Category => "Emby";

        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger logProgress)
        {
            return Task.Run(() =>
            {
               
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
