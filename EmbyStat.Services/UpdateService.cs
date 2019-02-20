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
using EmbyStat.Clients.Github;
using EmbyStat.Clients.Github.Models;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Model.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly ISettingsService _settingsService;
        private readonly IApplicationLifetime _applicationLifetime;

        public UpdateService(IGithubClient githubClient, ISettingsService settingsService, IApplicationLifetime appLifetime)
        {
            _githubClient = githubClient;
            _settingsService = settingsService;
            _applicationLifetime = appLifetime;
        }

        public async Task<UpdateResult> CheckForUpdate(CancellationToken cancellationToken)
        {
            try
            {
                var settings = _settingsService.GetUserSettings();
                return await CheckForUpdate(settings, cancellationToken);
            }
            catch (HttpException e)
            {
                throw new BusinessException("CANTCONNECTTOGITHUB", 500, e);
            }
        }


        public async Task<UpdateResult> CheckForUpdate(UserSettings settings, CancellationToken cancellationToken)
        {
            var appSettings = _settingsService.GetAppSettings();
            var currentVersion = new Version(appSettings.Version.ToCleanVersionString());
            var result = await _githubClient.CheckIfUpdateAvailable(currentVersion, appSettings.Updater.UpdateAsset, settings.UpdateTrain, cancellationToken);

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
            var fileName = $"{package.versionStr}.ver";
            var obj = JsonSerializerExtentions.SerializeToString(package);

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver"))
            {
                File.Delete(file);
            }
            
            File.WriteAllText($"{fileName}", obj);
        }

        public async Task DownloadZip(UpdateResult result)
        {
            var appSettings = _settingsService.GetAppSettings();
            if (Directory.Exists(appSettings.Dirs.TempUpdateDir))
            {
                Directory.Delete(appSettings.Dirs.TempUpdateDir, true);
            }
            Directory.CreateDirectory(appSettings.Dirs.TempUpdateDir);

            try
            {
                Log.Information("---------------------------------");
                Log.Information($"Downloading zip file {result.Package.name}");

                var webClient = new WebClient();
                webClient.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e) { DownloadFileCompleted(sender, e, result); };
                await webClient.DownloadFileTaskAsync(result.Package.sourceUrl, result.Package.name);
            }
            catch (Exception e)
            {
                Log.Error(e, "Downloading update zip failed!");
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, UpdateResult result)
        {
            Log.Information("Downloading finished");
            UnPackZip(result);
            CreateUpdateFile(result.Package);
        }

        private void UnPackZip(UpdateResult result)
        {
            Log.Information("---------------------------------");
            Log.Information("Starting to unpack new version");

            try
            {
                var appSettings = _settingsService.GetAppSettings();
                ZipFile.ExtractToDirectory(result.Package.name, appSettings.Dirs.TempUpdateDir, true);
            }
            catch (Exception e)
            {
                Log.Error("Unpack error", e);
                throw new BusinessException("UPDATEUNPACKERROR");
            }
            finally
            {
                File.Delete(result.Package.name);
            }
        }

        public async Task UpdateServer()
        {
            await Task.Run(() =>
            {
                Log.Information("Starting updater process.");
                var updateFile = CheckForUpdateFiles();
                if (updateFile != null)
                {
                    var appSettings = _settingsService.GetAppSettings();
                    Log.Information(Directory.GetCurrentDirectory());
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

                    Log.Information($"StartJob tool located at {updaterTool}");
                    Log.Information($"Arguments passed are {GetArgs(appSettings)}");
                    Log.Information($"Working directory is {workingDirectory}");

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

        private PackageInfo ReadUpdateFile(FileInfo file)
        {
            var json = File.ReadAllText(file.Name);
            return JsonSerializerExtentions.DeserializeFromString<PackageInfo>(json);
        }
    }
}