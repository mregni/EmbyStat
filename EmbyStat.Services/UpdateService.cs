using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Common.Net;
using MediaBrowser.Common.Updates;
using MediaBrowser.Model.Updates;
using Serilog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;

        public UpdateService(IGithubClient githubClient)
        {
            _githubClient = githubClient;
        }

        public async void CheckForUpdate()
        {
            var currentVersion = GetType().GetTypeInfo().Assembly.GetName().Version;
            var result = await _githubClient.CheckIfUpdateAvailable(currentVersion, "win10-x64-v{version}.zip", "EmbystatUpdate", "update.zip", new CancellationToken(false));

            if (result.IsUpdateAvailable)
            {
                //Notify everyone that there is an update
            }
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
