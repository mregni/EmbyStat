using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Maintenance
{
    [DisableConcurrentExecution(30)]
    public class PingEmbyJob : BaseJob, IPingEmbyJob
    {
        private readonly IMediaServerService _mediaServerService;

        public PingEmbyJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
            IMediaServerService mediaServerService) 
            : base(hubHelper, jobRepository, settingsService, false, typeof(PingEmbyJob), Constants.LogPrefix.PingMediaServerJob)
        {
            _mediaServerService = mediaServerService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.PingEmbyId;
        public override string JobPrefix => Constants.LogPrefix.PingMediaServerJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            var result = _mediaServerService.PingMediaServer(Settings.MediaServer.Address);
            await LogProgress(50);
            if (result)
            {
                await LogInformation("We found your MediaServer server");
                _mediaServerService.ResetMissedPings();
            }
            else
            {
                await LogInformation("We could not ping your MediaServer server. Might be because it's turned off or dns is wrong");
                _mediaServerService.IncreaseMissedPings();
            }

            var status = _mediaServerService.GetMediaServerStatus();
            await HubHelper.BroadcastEmbyConnectionStatus(status.MissedPings);

        }
    }
}