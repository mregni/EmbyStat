using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly AppSettings _appSettings;

        public UpdateService(IGithubClient githubClient, IOptions<AppSettings> appSettings, IJsonSerializer jsonSerializer)
        {
            _githubClient = githubClient;
            _jsonSerializer = jsonSerializer;
            _appSettings = appSettings.Value;
        }

        public async Task<CheckForUpdateResult> CheckForUpdate(CancellationToken cancellationToken)
        {
            var currentVersion = new Version(_appSettings.Version);
            var result = await _githubClient.CheckIfUpdateAvailable(currentVersion, _appSettings.UpdateAsset, cancellationToken);

            if (result.IsUpdateAvailable)
            {
                CreateUpdateFile(result.Package);
                //Notify everyone that there is an update
            }

            return result;
        }

        public void UpdateServer()
        {
            Log.Information("Starting updater process.");
            var updateFile = CheckForUpdateFiles();
            if (updateFile != null)
            {
                var package = ReadUpdateFile(updateFile);

                var args = $"{Process.GetCurrentProcess().Id} {package.sourceUrl} {package.name}";
                Log.Information("Args: {0}", args);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"../Updater/bin/Release/netcoreapp2.1/win-x64/Updater.dll {args}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                });
            }
        }

        private PackageInfo ReadUpdateFile(FileInfo file)
        {
            var json = File.ReadAllText(file.Name);
            return _jsonSerializer.DeserializeFromString<PackageInfo>(json);
        }

        private FileInfo CheckForUpdateFiles()
        {
            var fileNames = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver");
            return fileNames.Select(x => new FileInfo(x)).OrderByDescending(x => x.Name).FirstOrDefault();
        }

        private void CreateUpdateFile(PackageInfo package)
        {
            var fileName = $"{package.versionStr}.ver";
            var obj = _jsonSerializer.SerializeToString(package);

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver"))
            {
                File.Delete(file);
            }
            
            File.WriteAllText($"{fileName}", obj);
        }
    }
}