using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using EmbyStat.Clients.GitHub;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Updates.Interfaces;
using MediaBrowser.Model.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.Updates;

public class UpdateService : IUpdateService
{
    private readonly IGitHubClient _gitHubClient;
    private readonly IConfigurationService _configurationService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(IGitHubClient gitHubClient, IConfigurationService configurationService,
        IHostApplicationLifetime appLifetime, ILogger<UpdateService> logger)
    {
        _gitHubClient = gitHubClient;
        _configurationService = configurationService;
        _applicationLifetime = appLifetime;
        _logger = logger;
    }

    public async Task<UpdateResult> CheckForUpdate()
    {
        try
        {
            var config = _configurationService.Get().SystemConfig;
            var currentVersion = new Version(config.Version.ToCleanVersionString());
            var result = await _gitHubClient.GetGithubVersions();
            var update = CheckForUpdateResult(result, currentVersion, config.UpdateTrain,
                config.Updater.UpdateAsset);

            if (update.IsUpdateAvailable)
            {
                //TODO: Notify everyone that there is an update
            }

            return update;
        }
        catch (HttpException e)
        {
            throw new BusinessException("CANTCONNECTTOGITHUB", 500, e);
        }
    }

    #region CheckForUpdate

    private UpdateResult CheckForUpdateResult(IEnumerable<ReleaseObject> obj, Version minVersion,
        UpdateTrain updateTrain,
        string assetFilename)
    {
        var correctTrainReleases = Array.Empty<ReleaseObject>();
        var configuration = _configurationService.Get().SystemConfig;
        if (updateTrain == UpdateTrain.Release)
        {
            correctTrainReleases = obj.Where(i => !i.PreRelease).ToArray();
        }
        else if (updateTrain == UpdateTrain.Beta)
        {
            correctTrainReleases = obj.Where(i =>
                i.PreRelease && i.Name.Contains(configuration.Updater.BetaString,
                    StringComparison.OrdinalIgnoreCase)).ToArray();
        }
        else if (updateTrain == UpdateTrain.Dev)
        {
            correctTrainReleases = obj.Where(i =>
                i.PreRelease && i.Name.Contains(configuration.Updater.DevString,
                    StringComparison.OrdinalIgnoreCase)).ToArray();
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
            var configuration = _configurationService.Get().SystemConfig;
            if (obj.Name.Contains(configuration.Updater.DevString,
                    StringComparison.OrdinalIgnoreCase))
            {
                classification = UpdateTrain.Dev;
            }
            else if (obj.Name.Contains(configuration.Updater.BetaString,
                         StringComparison.OrdinalIgnoreCase))
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
        var configuration = _configurationService.Get().SystemConfig;
        return version
            .Replace(configuration.Updater.BetaString, "")
            .Replace(configuration.Updater.DevString, "");
    }

    #endregion

    #region UpdateApp

    public async Task DownloadZipAsync(UpdateResult result)
    {
        var configuration = _configurationService.Get().SystemConfig;
        if (Directory.Exists(configuration.Dirs.TempUpdate))
        {
            Directory.Delete(configuration.Dirs.TempUpdate, true);
        }

        Directory.CreateDirectory(configuration.Dirs.TempUpdate);

        try
        {
            _logger.LogInformation("---------------------------------");
            _logger.LogInformation($"Downloading zip file {result.Package.Name}");

            using var webClient = new WebClient();
            webClient.DownloadFileCompleted += delegate { DownloadFileCompleted(result); };
            await webClient.DownloadFileTaskAsync(result.Package.SourceUrl, result.Package.Name);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Downloading update zip failed!");
        }
    }

    public Task UpdateServerAsync()
    {
        return Task.Run(() =>
        {
            _logger.LogInformation("Starting updater process.");
            var configuration = _configurationService.Get();
            _logger.LogInformation(Directory.GetCurrentDirectory());
            var updaterExtension = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                updaterExtension = ".exe";
            }

            var localPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)
                            ?? Directory.GetCurrentDirectory();
            var updaterTool = Path.Combine(localPath, configuration.SystemConfig.Dirs.TempUpdate,
                configuration.SystemConfig.Dirs.Updater,
                $"Updater{updaterExtension}");
            var workingDirectory = Path.Combine(localPath, configuration.SystemConfig.Dirs.TempUpdate);

            if (!File.Exists(updaterTool))
            {
                _logger.LogDebug($"Update tool not found in location: {updaterTool}");
                throw new BusinessException("NOUPDATEFILE");
            }

            _logger.LogInformation($"StartJob tool located at {updaterTool}");
            _logger.LogInformation($"Arguments passed are {GetArgs(configuration)}");
            _logger.LogInformation($"Working directory is {workingDirectory}");

            var start = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = updaterTool,
                Arguments = GetArgs(configuration),
                WorkingDirectory = workingDirectory
            };

            using var proc = new Process {StartInfo = start};
            proc.Start();
            _applicationLifetime.StopApplication();
        });
    }

    private void DownloadFileCompleted(UpdateResult result)
    {
        _logger.LogInformation("Downloading finished");
        UnPackZip(result);
    }

    private void UnPackZip(UpdateResult result)
    {
        _logger.LogInformation("---------------------------------");
        _logger.LogInformation("Starting to unpack new version");

        try
        {
            var configuration = _configurationService.Get();
            ZipFile.ExtractToDirectory(
                result.Package.Name,
                configuration.SystemConfig.Dirs.TempUpdate,
                true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unpack error");
            throw new BusinessException("UPDATEUNPACKERROR");
        }
        finally
        {
            File.Delete(result.Package.Name);
        }
    }

    private static string GetArgs(Config configuration)
    {
        var currentLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)
                              ?? Directory.GetCurrentDirectory();

        var systemConfig = configuration.SystemConfig;
        var userConfig = configuration.UserConfig;
        var sb = new StringBuilder();
        sb.Append($"--applicationPath \"{currentLocation}\"");
        sb.Append($" --processId {Environment.ProcessId}");
        sb.Append($" --processName {systemConfig.ProcessName}");
        sb.Append($" --port {userConfig.Hosting.Port}");
        sb.Append($" --listening-urls {userConfig.Hosting.Url}");

        return sb.ToString();
    }
}

#endregion