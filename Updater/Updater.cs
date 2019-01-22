using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;
using Updater.Models;

namespace Updater
{
    public class Updater
    {
        private readonly StartupOptions _options;

        public Updater(StartupOptions options)
        {
            _options = options;
        }

        public void Start()
        {
            Console.WriteLine("Start killing EmbyStat now");
            var state = KillProcess();
            if (!state)
            {
                Log.Warning("Couldn't kill the EmbyStat process");
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
                Console.WriteLine($"Cannot find process with name: {_options.ProcessName}");
                return false;
            }

            if (process.Id > 0)
            {
                Console.WriteLine($"{process.Id}: Killing process");
                process.Kill();
                process.WaitForExit();
                Console.WriteLine($"{process.Id}: Process terminated successfully");


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
                Log.Debug($"Created dir {newDir}");
            }

            foreach (string currentPath in Directory.GetFiles(updatedLocation, "*.*", SearchOption.AllDirectories))
            {
                var newFile = currentPath.Replace(updatedLocation, _options.ApplicationPath);
                File.Copy(currentPath, newFile, true);
                Log.Debug($"Replaced file {newFile}");
            }
        }

        private void StartEmbyStat()
        {
            Log.Debug("Starting EmbyStat");
            var processName = _options.ProcessName;

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

            Log.Debug($"EmbyStat started, now exiting");
            Log.Debug($"Working dir: {_options.ApplicationPath} (Application Path)");
            Log.Debug($"Filename: {Path.Combine(_options.ApplicationPath, processName)}");

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