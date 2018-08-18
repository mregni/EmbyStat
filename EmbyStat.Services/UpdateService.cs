using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly AppSettings _appSettings;

        public UpdateService(IGithubClient githubClient, IOptions<AppSettings> appSettings)
        {
            _githubClient = githubClient;
            _appSettings = appSettings.Value;
        }

        public async Task<CheckForUpdateResult> CheckForUpdate(CancellationToken cancellationToken)
        {
            var currentVersion = new Version(_appSettings.Version);
            var result = await _githubClient.CheckIfUpdateAvailable(currentVersion, _appSettings.UpdateAsset, cancellationToken);

            if (result.IsUpdateAvailable)
            {
                //Notify everyone that there is an update
                //Save update info to database
            }

            return result;
        }

        public void UpdateServer()
        {
            Log.Information("Starting updater process.");

            var args = $"callerId={Process.GetCurrentProcess().Id}";
            Log.Information("Args: {0}", args);

            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "../Updater/bin/Release/netcoreapp2.1/win10-x64/publish/Updater.dll",
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory()
            });
        }
    }
}
