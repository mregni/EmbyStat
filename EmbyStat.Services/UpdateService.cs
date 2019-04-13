using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Model.Net;
using Microsoft.AspNetCore.Hosting;
using NLog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly ISettingsService _settingsService;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly Logger _logger;

        public UpdateService(IGithubClient githubClient, ISettingsService settingsService, IApplicationLifetime appLifetime)
        {
            _githubClient = githubClient;
            _settingsService = settingsService;
            _applicationLifetime = appLifetime;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<UpdateResult> CheckForUpdateAsync(CancellationToken cancellationToken)
        {
            try
            {
                var settings = _settingsService.GetUserSettings();
                return await CheckForUpdateAsync(settings, cancellationToken);
            }
            catch (HttpException e)
            {
                throw new BusinessException("CANTCONNECTTOGITHUB", 500, e);
            }
        }


        public async Task<UpdateResult> CheckForUpdateAsync(UserSettings settings, CancellationToken cancellationToken)
        {
            var appSettings = _settingsService.GetAppSettings();
            var currentVersion = new Version(appSettings.Version.ToCleanVersionString());
            var result = await _githubClient.CheckIfUpdateAvailableAsync(currentVersion, appSettings.Updater.UpdateAsset, settings.UpdateTrain, cancellationToken);

            if (result.IsUpdateAvailable)
            {
                //Notify everyone that there is an update
            }

            return result;
        }

        private FileInfo CheckForUpdateFiles()
        {
            var fileNames = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver");
            return fileNames.Select(x => new FileInfo(x)).OrderByDescending(x => x.Name).FirstOrDefault();
        }

        private void CreateUpdateFile(PackageInfo package)
        {
            var fileName = $"{package.VersionStr}.ver";
            var obj = JsonSerializerExtentions.SerializeToString(package);

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver"))
            {
                File.Delete(file);
            }
            
            File.WriteAllText($"{fileName}", obj);
        }

        public async Task DownloadZipAsync(UpdateResult result)
        {
            var appSettings = _settingsService.GetAppSettings();
            if (Directory.Exists(appSettings.Dirs.TempUpdateDir))
            {
                Directory.Delete(appSettings.Dirs.TempUpdateDir, true);
            }
            Directory.CreateDirectory(appSettings.Dirs.TempUpdateDir);

            try
            {
                _logger.Info("---------------------------------");
                _logger.Info($"Downloading zip file {result.Package.Name}");

                var webClient = new WebClient();
                webClient.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e) { DownloadFileCompleted(sender, e, result); };
                await webClient.DownloadFileTaskAsync(result.Package.SourceUrl, result.Package.Name);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Downloading update zip failed!");
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, UpdateResult result)
        {
            _logger.Info("Downloading finished");
            UnPackZip(result);
            CreateUpdateFile(result.Package);
        }

        private void UnPackZip(UpdateResult result)
        {
            _logger.Info("---------------------------------");
            _logger.Info("Starting to unpack new version");

            try
            {
                var appSettings = _settingsService.GetAppSettings();
                ZipFile.ExtractToDirectory(result.Package.Name, appSettings.Dirs.TempUpdateDir, true);
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

        public Task UpdateServerAsync()
        {
            return Task.Run(() =>
            {
                _logger.Info("Starting updater process.");
                var updateFile = CheckForUpdateFiles();
                if (updateFile != null)
                {
                    var appSettings = _settingsService.GetAppSettings();
                    _logger.Info(Directory.GetCurrentDirectory());
                    var updaterExtension = string.Empty;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        updaterExtension = ".exe";
                    }
                    var updaterTool = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), appSettings.Dirs.TempUpdateDir, appSettings.Dirs.Updater, $"Updater{updaterExtension}");
                    var workingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), appSettings.Dirs.TempUpdateDir);

                    if (!File.Exists(updaterTool))
                    {
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

                    using (var proc = new Process { StartInfo = start })
                    {
                        proc.Start();
                        _applicationLifetime.StopApplication();
                    }
                }
            });
        }

        private string GetArgs(AppSettings appSettings)
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var sb = new StringBuilder();
            sb.Append($"--applicationPath \"{currentLocation}\"");
            sb.Append($" --processId {Process.GetCurrentProcess().Id}");
            sb.Append($" --processName {appSettings.ProcessName}");
            sb.Append($" --port {appSettings.Port}");

            return sb.ToString();
        }
    }
}