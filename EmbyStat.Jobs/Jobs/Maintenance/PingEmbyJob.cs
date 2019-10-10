using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Maintenance
{
    [DisableConcurrentExecution(30)]
    public class PingEmbyJob : BaseJob, IPingEmbyJob
    {
        private readonly IEmbyService _embyService;

        public PingEmbyJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
            IEmbyService embyService) : base(hubHelper, jobRepository, settingsService, false)
        {
            _embyService = embyService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.PingEmbyId;
        public override string JobPrefix => Constants.LogPrefix.PingEmbyJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            var result = await _embyService.PingEmbyAsync(Settings.Emby.FullEmbyServerAddress);
            await LogProgress(50);
            if (result == "Emby Server")
            {
                await LogInformation("We found your Emby server");
                _embyService.ResetMissedPings();
            }
            else
            {
                await LogInformation("We could not ping your Emby server. Might be because it's turned off or dns is wrong");
                _embyService.IncreaseMissedPings();
            }

            var status = _embyService.GetEmbyStatus();
            await HubHelper.BroadcastEmbyConnectionStatus(status.MissedPings);

        }

        public void Dispose()
        {
            _embyService.Dispose();
        }
    }
}