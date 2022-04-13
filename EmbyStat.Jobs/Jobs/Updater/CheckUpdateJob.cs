﻿using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Jobs.Jobs.Updater;

[DisableConcurrentExecution(30)]
public class CheckUpdateJob : BaseJob, ICheckUpdateJob
{
    private readonly IUpdateService _updateService;
    private readonly ISettingsService _settingsService;

    public CheckUpdateJob(IHubHelper hubHelper, IJobRepository jobRepository,
        ISettingsService settingsService, IUpdateService updateService, ILogger<CheckUpdateJob> logger) 
        : base(hubHelper, jobRepository, settingsService, logger)
    {
        _updateService = updateService;
        _settingsService = settingsService;
    }

    protected sealed override Guid Id => Constants.JobIds.CheckUpdateId;
    protected override string JobPrefix => Constants.LogPrefix.CheckUpdateJob;

    protected override async Task RunJobAsync()
    {
        try
        {
            await LogInformation("Contacting Github now to see if new version is available.");
            var update = await _updateService.CheckForUpdate();
            await LogProgress(20);
            if (update.IsUpdateAvailable && Settings.AutoUpdate)
            {
                await LogInformation($"New version found: v{update.AvailableVersion}");
                await LogInformation("Auto update is enabled so going to update the server now!");
                await _settingsService.SetUpdateInProgressSettingAsync(true);
                await HubHelper.BroadcastUpdateState(true);
                Task.WaitAll(_updateService.DownloadZipAsync(update));
                await LogProgress(50);
                await _updateService.UpdateServerAsync();
                await _settingsService.SetUpdateInProgressSettingAsync(false);
                await HubHelper.BroadcastUpdateFinished(true);
            }
            else if (update.IsUpdateAvailable)
            {
                await LogInformation($"New version found: v{update.AvailableVersion}");
                await LogInformation("Auto updater is disabled, so going to end the job now.");
            }
            else
            {
                await LogInformation("No new version available");
            }
        }
        catch (Exception)
        {
            await _settingsService.SetUpdateInProgressSettingAsync(false);
            await HubHelper.BroadcastUpdateState(false);
            await HubHelper.BroadcastUpdateFinished(false);
            throw;
        }
    }
}