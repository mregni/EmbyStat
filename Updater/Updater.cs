using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Serilog;
using Updater.Models;

namespace Updater
{
    public class Updater
    {
        private readonly StartupOptions _options;
        private const string UpdateTempFolder = "update";

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
                Console.WriteLine($"Cannot find process with name: {process.ProcessName}");
                return false;
            }
            else if (process.Id > 0)
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

            try
            {
		foreach (string dirPath in Directory.GetDirectories(updatedLocation, "*", SearchOption.AllDirectories))
                {
                	var newDir = dirPath.Replace(updatedLocation, _options.ApplicationPath);
                	Directory.CreateDirectory(newDir);
                	Log.Debug($"Created dir {newDir}");
                }
            
                foreach (var file in Directory.GetFiles(UpdateTempFolder, "*.*", SearchOption.AllDirectories))
                {
                	var newFile = currentPath.Replace(updatedLocation, _options.ApplicationPath);
                	File.Copy(currentPath, newFile, true);
                	Log.Debug($"Replaced file {newFile}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error("error", e);
                Log.Error(e.Message);
                throw;
            }

        }

        private void StartEmbyStat()
        {
            Log.Debug("Starting EmbyStat");
            var fileName = "EmbyStat.Web";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName += ".exe";
            }

            var start = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = Path.Combine(_options.ApplicationPath, fileName),
                WorkingDirectory = _options.ApplicationPath,
                Arguments = ""
            };

            using (var proc = new Process { StartInfo = start })
            {
                proc.Start();
            }

            Log.Debug($"EmbyStat started, now exiting");
            Log.Debug($"Working dir: {_options.ApplicationPath} (Application Path)");
            Log.Debug($"Filename: {Path.Combine(_options.ApplicationPath, fileName)}");
            Environment.Exit(0);

        }
    }
}
