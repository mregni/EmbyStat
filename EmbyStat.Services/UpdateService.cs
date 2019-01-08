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
using EmbyStat.Api.Github;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace EmbyStat.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IGithubClient _githubClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly AppSettings _appSettings;

        public UpdateService(IGithubClient githubClient, IOptions<AppSettings> appSettings, IJsonSerializer jsonSerializer, IConfigurationRepository configurationRepository)
        {
            _githubClient = githubClient;
            _jsonSerializer = jsonSerializer;
            _appSettings = appSettings.Value;
            _configurationRepository = configurationRepository;
        }

        public async Task<UpdateResult> CheckForUpdate(CancellationToken cancellationToken)
        {
            var settings = _configurationRepository.GetConfiguration();
            return await CheckForUpdate(settings, cancellationToken);
        }


        public async Task<UpdateResult> CheckForUpdate(Configuration settings, CancellationToken cancellationToken)
        {
            var currentVersion = new Version(_appSettings.Version);
            var result = await _githubClient.CheckIfUpdateAvailable(currentVersion, _appSettings.Updater.UpdateAsset, settings.UpdateTrain, cancellationToken);

            Log.Debug($"result is {result.IsUpdateAvailable} {result.Package.name}");
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
            var obj = _jsonSerializer.SerializeToString(package);

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ver"))
            {
                File.Delete(file);
            }
            
            File.WriteAllText($"{fileName}", obj);
        }

        public async Task DownloadZip(UpdateResult result)
        {
            if (Directory.Exists(_appSettings.Dirs.TempUpdateDir))
            {
                Directory.Delete(_appSettings.Dirs.TempUpdateDir, true);
            }
            Directory.CreateDirectory(_appSettings.Dirs.TempUpdateDir);

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
                ZipFile.ExtractToDirectory(result.Package.name, _appSettings.Dirs.TempUpdateDir, true);
            }
            catch (Exception e)
            {
                Log.Error("Unpack error", e);
            }

            File.Delete(result.Package.name);
        }

        public async Task UpdateServer()
        {
            await Task.Run(() =>
            {
                Log.Information("Starting updater process.");
                var updateFile = CheckForUpdateFiles();
                if (updateFile != null)
                {
                    var package = ReadUpdateFile(updateFile);
                    Log.Information(Directory.GetCurrentDirectory());
                    var updaterExtension = string.Empty;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        updaterExtension = ".exe";
                    }
                    var updaterTool = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _appSettings.Dirs.TempUpdateDir, _appSettings.Dirs.Updater, $"Updater{updaterExtension}");
                    var workingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _appSettings.Dirs.TempUpdateDir);

                    Log.Information($"Update tool located at {updaterTool}");
                    Log.Information($"Arguments passed are {GetArgs()}");
                    Log.Information($"Working directory is {workingDirectory}");

                    var start = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = updaterTool,
                        Arguments = GetArgs(),
                        WorkingDirectory = workingDirectory
                    };

                    using (var proc = new Process { StartInfo = start })
                    {
                        proc.Start();
                    }
                }
            });
        }

        private string GetArgs()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var sb = new StringBuilder();
            sb.Append($"--applicationPath \"{currentLocation}\"");
            sb.Append($" --processId {Process.GetCurrentProcess().Id}");
            sb.Append($" --processName {_appSettings.ProcessName}");

            return sb.ToString();
        }

        private PackageInfo ReadUpdateFile(FileInfo file)
        {
            var json = File.ReadAllText(file.Name);
            return _jsonSerializer.DeserializeFromString<PackageInfo>(json);
        }
    }
}