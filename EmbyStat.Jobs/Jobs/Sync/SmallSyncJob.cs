﻿using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Jobs.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Sync;

[DisableConcurrentExecution(60)]
public class SmallSyncJob : BaseJob, ISmallSyncJob
{
    private readonly IMediaServerService _mediaServerService;

    public SmallSyncJob(IHubHelper hubHelper, IJobRepository jobRepository, IConfigurationService configurationService, 
        IMediaServerService mediaServerService, ILogger<SmallSyncJob> logger) 
        : base(hubHelper, jobRepository, configurationService, logger)
    {
        _mediaServerService = mediaServerService;
    }

    protected sealed override Guid Id => Constants.JobIds.SmallSyncId;
    protected override string JobPrefix => Constants.LogPrefix.SmallMediaServerSyncJob;

    protected override async Task RunJobAsync()
    {
        await _mediaServerService.GetAndProcessServerInfo();
        await LogInformation("Server info downloaded");
        await LogProgress(35);

        await _mediaServerService.GetAndProcessPluginInfo();
        await LogInformation("Server plugins downloaded");
        await LogProgress(45);

        await _mediaServerService.GetAndProcessUsers();
        await LogInformation("Server users downloaded");
        await LogProgress(55);

        await ProcessUserViews();
        await LogProgress(80);

        await _mediaServerService.GetAndProcessDevices();
        await LogInformation("Server devices downloaded");
        await LogProgress(90);

        await _mediaServerService.GetAndProcessLibraries();
        await LogInformation("Server libraries downloaded");
    }

    private async Task ProcessUserViews()
    {
        var users = await _mediaServerService.GetAllUsers();
        var stepPerUser = 25d / users.Length;
        foreach (var user in users)
        {
            var viewCount = await _mediaServerService.ProcessViewsForUser(user.Id);
            await LogInformation($"Processed {viewCount} views for {user.Name}");
            await LogProgressIncrement(stepPerUser);
        }

        await LogInformation($"Calculating user statistics");
        await _mediaServerService.CalculateMediaServerUserStatistics();
    }
}