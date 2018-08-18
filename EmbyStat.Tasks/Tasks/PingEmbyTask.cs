using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Interface;
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
        private readonly IEmbyService _embyService;
        public PingEmbyTask(IApplicationBuilder app)
        {
            _embyStatusRepository =  app.ApplicationServices.GetService<IEmbyStatusRepository>();
            _embyService = app.ApplicationServices.GetService<IEmbyService>();
        }

        public string Name => "TASKS.PINGEMBYSERVERTITLE";
        public string Key => "PingEmbyServer";
        public string Description => "TASKS.PINGEMBYSERVERDESCRIPTION";
        public string Category => "Emby";

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgressLogger logProgress)
        {
            progress.Report(0);
            var result = await _embyService.PingEmbyAsync(cancellationToken);
            progress.Report(50);
            if (result == "Emby Server")
            {
                logProgress.LogInformation(Constants.LogPrefix.PingEmbyTask, "We found your Emby server");
                _embyStatusRepository.ResetMissedPings();
            }
            else
            {
                logProgress.LogInformation(Constants.LogPrefix.PingEmbyTask, "We could not ping your Emby server. Might be because it's turned off or dns is wrong");
                _embyStatusRepository.IncreaseMissedPings();
            }
            progress.Report(100);
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
