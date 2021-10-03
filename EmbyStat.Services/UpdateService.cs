﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Logging;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Model.Net;
using Microsoft.Extensions.Hosting;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly ISettingsService _settingsService;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly Logger _logger;

        public UpdateService(IGithubClient githubClient, ISettingsService settingsService, IHostApplicationLifetime appLifetime)
        {
            _githubClient = githubClient;
            _settingsService = settingsService;
            _applicationLifetime = appLifetime;
            _logger = LogFactory.CreateLoggerForType(typeof(UpdateService), "UPDATE-SERVICE");
        }

        public UpdateResult CheckForUpdate()
        {
            try
            {
                var settings = _settingsService.GetUserSettings();
                return CheckForUpdate(settings);
            }
            catch (HttpException e)
            {
                throw new BusinessException("CANTCONNECTTOGITHUB", 500, e);
            }
        }

        #region CheckForUpdate

        public UpdateResult CheckForUpdate(UserSettings settings)
        {
            var appSettings = _settingsService.GetAppSettings();
            var currentVersion = new Version(appSettings.Version.ToCleanVersionString());
            var result = _githubClient.GetGithubVersions(currentVersion, appSettings.Updater.UpdateAsset, settings.UpdateTrain);
            var update = CheckForUpdateResult(result, currentVersion, settings.UpdateTrain, appSettings.Updater.UpdateAsset);

            if (update.IsUpdateAvailable)
            {
                //TODO: Notify everyone that there is an update
            }

            return update;
        }

        public UpdateResult CheckForUpdateResult(ReleaseObject[] obj, Version minVersion, UpdateTrain updateTrain, string assetFilename)
        {
            ReleaseObject[] correctTrainReleases = Array.Empty<ReleaseObject>();
            if (updateTrain == UpdateTrain.Release)
            {
                correctTrainReleases = obj.Where(i => !i.PreRelease).ToArray();
            }
            else if (updateTrain == UpdateTrain.Beta)
            {
                correctTrainReleases = obj.Where(i => i.PreRelease && i.Name.Contains(_settingsService.GetAppSettings().Updater.BetaString, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else if (updateTrain == UpdateTrain.Dev)
            {
                correctTrainReleases = obj.Where(i => i.PreRelease && i.Name.Contains(_settingsService.GetAppSettings().Updater.DevString, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            var availableUpdate = correctTrainReleases
                .Select(i => CheckForUpdateResult(i, minVersion, assetFilename))
                .Where(i => i != null)
                .OrderByDescending(i => Version.Parse(i.AvailableVersion))
                .FirstOrDefault();

            return availableUpdate ?? new UpdateResult();
        }

        private UpdateResult CheckForUpdateResult(ReleaseObject obj, Version minVersion, string assetFilename)
        {
            var versionString = CleanUpVersionString(obj.TagName);

            if (!Version.TryParse(versionString, out var version))
            {
                return null;
            }

            if (version < minVersion)
            {
                return null;
            }

            var asset = (obj.Assets ?? new List<Asset>()).FirstOrDefault(i => IsAsset(i, assetFilename, obj.TagName));
            if (asset == null)
            {
                return null;
            }

            UpdateTrain classification;
            if (obj.PreRelease)
            {
                if (obj.Name.Contains(_settingsService.GetAppSettings().Updater.DevString, StringComparison.OrdinalIgnoreCase))
                {
                    classification = UpdateTrain.Dev;
                }
                else if (obj.Name.Contains(_settingsService.GetAppSettings().Updater.BetaString, StringComparison.OrdinalIgnoreCase))
                {
                    classification = UpdateTrain.Beta;
                }
                else
                {
                    classification = UpdateTrain.Release;
                }
            }
            else
            {
                classification = UpdateTrain.Release;
            }

            return new UpdateResult
            {
                AvailableVersion = version.ToString(),
                IsUpdateAvailable = version > minVersion,
                Package = new PackageInfo
                {
                    Classification = classification,
                    Name = asset.Name,
                    SourceUrl = asset.BrowserDownloadUrl,
                    VersionStr = version.ToString(),
                    InfoUrl = obj.HtmlUrl
                }
            };
        }

        private static bool IsAsset(Asset asset, string assetFilename, string version)
        {
            var downloadFilename = Path.GetFileName(asset.Name) ?? string.Empty;
            var fullAssetFilename = assetFilename.Replace("{version}", version);

            return string.Equals(fullAssetFilename, downloadFilename, StringComparison.OrdinalIgnoreCase);
        }

        private string CleanUpVersionString(string version)
        {
            return version.Replace(_settingsService.GetAppSettings().Updater.BetaString, "").Replace(_settingsService.GetAppSettings().Updater.DevString, "");
        }

        #endregion

        #region UpdateApp

        public async Task DownloadZipAsync(UpdateResult result)
        {
            var appSettings = _settingsService.GetAppSettings();
            if (Directory.Exists(appSettings.Dirs.TempUpdate))
            {
                Directory.Delete(appSettings.Dirs.TempUpdate, true);
            }
            Directory.CreateDirectory(appSettings.Dirs.TempUpdate);

            try
            {
                _logger.Info("---------------------------------");
                _logger.Info($"Downloading zip file {result.Package.Name}");

                using var webClient = new WebClient();
                webClient.DownloadFileCompleted += delegate { DownloadFileCompleted(result); };
                await webClient.DownloadFileTaskAsync(result.Package.SourceUrl, result.Package.Name);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Downloading update zip failed!");
            }
        }

        public Task UpdateServerAsync()
        {
            return Task.Run(() =>
            {
                _logger.Info("Starting updater process.");
                var appSettings = _settingsService.GetAppSettings();
                _logger.Info(Directory.GetCurrentDirectory());
                var updaterExtension = string.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    updaterExtension = ".exe";
                }
                var updaterTool = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), appSettings.Dirs.TempUpdate, appSettings.Dirs.Updater, $"Updater{updaterExtension}");
                var workingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), appSettings.Dirs.TempUpdate);

                if (!File.Exists(updaterTool))
                {
                    _logger.Debug($"Update tool not found in location: {updaterTool}");
                    throw new BusinessException("NOUPDATEFILE");
                }

                _logger.Info($"StartJob tool located at {updaterTool}");
                _logger.Info($"Arguments passed are {GetArgs(appSettings)}");
                _logger.Info($"Working directory is {workingDirectory}");

                var start = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = updaterTool,
                    Arguments = GetArgs(appSettings),
                    WorkingDirectory = workingDirectory
                };

                using var proc = new Process { StartInfo = start };
                proc.Start();
                _applicationLifetime.StopApplication();
            });
        }

        private void DownloadFileCompleted(UpdateResult result)
        {
            _logger.Info("Downloading finished");
            UnPackZip(result);
        }

        private void UnPackZip(UpdateResult result)
        {
            _logger.Info("---------------------------------");
            _logger.Info("Starting to unpack new version");

            try
            {
                var appSettings = _settingsService.GetAppSettings();
                ZipFile.ExtractToDirectory(result.Package.Name, appSettings.Dirs.TempUpdate, true);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unpack error");
                throw new BusinessException("UPDATEUNPACKERROR");
            }
            finally
            {
                File.Delete(result.Package.Name);
            }
        }

        private string GetArgs(AppSettings appSettings)
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var sb = new StringBuilder();
            sb.Append($"--applicationPath \"{currentLocation}\"");
            sb.Append($" --processId {Process.GetCurrentProcess().Id}");
            sb.Append($" --processName {appSettings.ProcessName}");
            sb.Append($" --port {appSettings.Port}");
            sb.Append($" --listening-urls {appSettings.ListeningUrls}");

            return sb.ToString();
        }
    }

    #endregion

}