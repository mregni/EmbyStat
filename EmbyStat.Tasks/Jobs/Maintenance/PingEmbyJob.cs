using System;
using System.Threading;
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
        private readonly IEmbyStatusRepository _embyStatusRepository;
        private readonly IEmbyService _embyService;

        public PingEmbyJob(IJobHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService, 
            IEmbyStatusRepository embyStatusRepository, IEmbyService embyService) : base(hubHelper, jobRepository, configurationService)
        {
            _embyStatusRepository = embyStatusRepository;
            _embyService = embyService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.PingEmbyId;
        public override string JobPrefix => Constants.LogPrefix.PingEmbyJob;
        public override string Title { get; }

        public override async Task RunJob()
        {
            var result = await _embyService.PingEmbyAsync(new CancellationToken(false));
            LogProgress(50);
            if (result == "Emby Server")
            {
                LogInformation("We found your Emby server");
                _embyStatusRepository.ResetMissedPings();
            }
            else
            {
                LogInformation("We could not ping your Emby server. Might be because it's turned off or dns is wrong");
                _embyStatusRepository.IncreaseMissedPings();
            }

            var status = _embyStatusRepository.GetEmbyStatus();
            await _hubHelper.BroadcastEmbyConnectionStatus(status.MissedPings);

        }

        public override void Dispose()
        {
        }
    }
}