using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rollbar;

namespace EmbyStat.Services;

public class SettingsService : ISettingsService
{
    private Configuration _configuration;

    public SettingsService(IOptions<Configuration> options)
    {
        _configuration = options.Value;
    }

    public void CreateRollbarLogger()
    {
        var rollbarConfig = new RollbarConfig(_configuration.Rollbar.AccessToken)
        {
            Environment = _configuration.Rollbar.Environment,
            MaxReportsPerMinute = _configuration.Rollbar.MaxReportsPerMinute,
            ReportingQueueDepth = _configuration.Rollbar.ReportingQueueDepth,
            Enabled = _configuration.EnableRollbarLogging,
            Transform = payload =>
            {
                payload.Data.CodeVersion = _configuration.Version;
                payload.Data.Custom = new Dictionary<string, object>
                {
                    {"Framework", RuntimeInformation.FrameworkDescription},
                    {"OS", RuntimeInformation.OSDescription}
                };
            }
        };

        RollbarLocator.RollbarInstance.Configure(rollbarConfig);
    }
}