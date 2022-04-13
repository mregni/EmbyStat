using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Maintenance;

[DisableConcurrentExecution(30)]
public class PingEmbyJob : BaseJob, IPingEmbyJob
{
    private readonly IMediaServerService _mediaServerService;

    public PingEmbyJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService,
        IMediaServerService mediaServerService, ILogger<PingEmbyJob> logger)
        : base(hubHelper, jobRepository, settingsService, false, logger)
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
            await LogWarning(
                $"We could not ping your MediaServer server at {Settings.MediaServer.Address}. Might be because it's turned off or dns is wrong");
            await _mediaServerService.IncreaseMissedPings();
        }

        var status = await _mediaServerService.GetMediaServerStatus();
        await HubHelper.BroadcastEmbyConnectionStatus(status.MissedPings);
    }
}