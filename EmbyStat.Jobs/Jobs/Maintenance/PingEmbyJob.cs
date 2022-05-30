using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Maintenance;

[DisableConcurrentExecution(30)]
public class PingEmbyJob : BaseJob, IPingEmbyJob
{
    private readonly IMediaServerService _mediaServerService;

    public PingEmbyJob(IHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService,
        IMediaServerService mediaServerService, ILogger<PingEmbyJob> logger)
        : base(hubHelper, jobRepository, configurationService, false, logger)
    {
        _mediaServerService = mediaServerService;
    }

    protected sealed override Guid Id => Constants.JobIds.PingEmbyId;
    protected override string JobPrefix => Constants.LogPrefix.PingMediaServerJob;

    protected override async Task RunJobAsync()
    {
        var result = await _mediaServerService.PingMediaServer();
        await LogProgress(50);
        if (result)
        {
            await LogInformation("We found your MediaServer server");
            await _mediaServerService.ResetMissedPings();
        }
        else
        {
            var address = Configuration.UserConfig.MediaServer.Address;
            await LogWarning(
                $"We could not ping your MediaServer server at {address}. Might be because it's turned off or dns is wrong");
            await _mediaServerService.IncreaseMissedPings();
        }

        var status = await _mediaServerService.GetMediaServerStatus();
        await HubHelper.BroadcastEmbyConnectionStatus(status.MissedPings);
    }
}