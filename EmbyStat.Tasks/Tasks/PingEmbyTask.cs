using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EmbyStat.Tasks.Tasks
{
    public class PingEmbyTask : IScheduledTask
    {
        private readonly IEmbyStatusRepository _embyStatusRepository;
        public PingEmbyTask(IApplicationBuilder app)
        {
            _embyStatusRepository =  app.ApplicationServices.GetService<IEmbyStatusRepository>();
        }

        public string Name => "TASKS.PINGEMBYSERVERTITLE";
        public string Key => "PingEmbyServer";
        public string Description => "TASKS.PINGEMBYSERVERDESCRIPTION";
        public string Category => "Emby";

        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger logProgress)
        {
            return Task.Run(() =>
            {
                progress.Report(0);
                logProgress.LogInformation(Constants.LogPrefix.PingEmbyTask, "Let's see if Emby is still online");
                _embyStatusRepository.IncreaseMissedPings();
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
