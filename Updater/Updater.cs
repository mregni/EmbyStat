using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using Updater.Models;

namespace Updater
{
    public class Updater
    {
        private readonly StartupOptions _options;
        private readonly Logger _logger;

        public Updater(StartupOptions options)
        {
            _options = options;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Start()
        {
            _logger.Info("Start killing EmbyStat now");
            var state = KillProcess();
            if (!state)
            {
                _logger.Warn("Couldn't kill the EmbyStat process");
                return;
            }

            MoveUpdate();
            StartEmbyStat();
        }

        private bool KillProcess()
        {
            var process = Process.GetProcesses().FirstOrDefault(p => p.Id == _options.ProcessId);
            if (process == null)
            {
                _logger.Info($"Cannot find process with name: {_options.ProcessName}");
                return true;
            }

            if (process.Id > 0)
            {
                _logger.Info($"{process.Id}: Killing process");
                process.Kill();
                process.WaitForExit();
                _logger.Info($"{process.Id}: Process terminated successfully");

                return true;
            }

            return false;
        }

        private void MoveUpdate()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            location = Path.GetDirectoryName(location);
            var updatedLocation = Directory.GetParent(location).FullName;

            Console.WriteLine(location);
            Console.WriteLine(updatedLocation);
            Console.WriteLine(_options.ApplicationPath);

            foreach (string dirPath in Directory.GetDirectories(updatedLocation, "*", SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(updatedLocation, _options.ApplicationPath);
                Directory.CreateDirectory(newDir);
                _logger.Debug($"Created dir {newDir}");
            }

            foreach (string currentPath in Directory.GetFiles(updatedLocation, "*.*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(currentPath);
                if (fileName != "data.db" || fileName != "usersettings.json")
                {
                    var newFile = currentPath.Replace(updatedLocation, _options.ApplicationPath);
                    File.Copy(currentPath, newFile, true);
                    _logger.Debug($"Replaced file {newFile}");
                }
            }
        }

        private void StartEmbyStat()
        {
            _logger.Info("Starting EmbyStat");
            var processName = _options.ProcessName.Replace(".Web", "");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                processName += ".exe";
            }

            var start = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = Path.Combine(_options.ApplicationPath, processName),
                WorkingDirectory = _options.ApplicationPath,
                Arguments = GetArgs()
            };

            using (var proc = new Process { StartInfo = start })
            {
                proc.Start();
            }

            _logger.Info($"EmbyStat started, now exiting");
            _logger.Info($"Working dir: {_options.ApplicationPath} (Application Path)");
            _logger.Info($"Filename: {Path.Combine(_options.ApplicationPath, processName)}");

            Environment.Exit(0);
        }

        private string GetArgs()
        {
            var sb = new StringBuilder();
            sb.Append($"--port {_options.Port}");

            return sb.ToString();
        }
    }
}