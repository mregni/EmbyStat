using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Updater
{
    public class Updater
    {
        private readonly string _updateUrl;
        private readonly string _updateFileName;
        private readonly int _processId;
        private const string UpdateTempFolder = "update";

        public Updater(string updateUrl, string updateFileName, int processId)
        {
            _updateUrl = updateUrl;
            _updateFileName = updateFileName;
            _processId = processId;
        }

        public bool Start()
        {
            RemoveUpdateLeftovers();

            if (!KillProcess()) return true;
            return !DownloadZip();
        }

        private bool KillProcess()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (KillProcessAndWait())
            {
                stopwatch.Stop();
                Log.Information($"EmbyStat server stopped after {stopwatch.Elapsed.TotalSeconds} seconds");
                return true;
            }
            else
            {
                stopwatch.Stop();
                Log.Warning("Halting updater, stopping EmbyStat took more then 20 seconds!");
                return false;
            }
        }
        
        private bool DownloadZip()
        {
            try
            {
                Log.Information("---------------------------------");
                Log.Information($"Downloading zip file {_updateFileName}");

                var webClient = new WebClient();
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                var task = webClient.DownloadFileTaskAsync(_updateUrl, _updateFileName);
                Task.WaitAll(task);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Downloading update zip failed!");
                return false;
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Log.Information("Downloading finished");
            UnPackZip();
            MoveFiles();
            RemoveUpdateLeftovers();
        }

        private void UnPackZip()
        {
            Log.Information("---------------------------------");
            Log.Information("Starting to unpack new version");
            try
            {
                if (!Directory.Exists(UpdateTempFolder))
                {
                    Directory.CreateDirectory(UpdateTempFolder);
                }

                ZipFile.ExtractToDirectory(_updateFileName, UpdateTempFolder, true);
            }
            catch (Exception e)
            {
                Log.Error("Unpack error", e);
            }

        }

        private static void MoveFiles()
        {
            Log.Information("---------------------------------");
            Log.Information("Starting moving files");

            foreach (var dir in Directory.GetDirectories(UpdateTempFolder, "*", SearchOption.AllDirectories))
            {
                Log.Information($"Creating folder {dir}");
                Directory.CreateDirectory(dir.Replace($"{UpdateTempFolder}\\", ""));
            }

            foreach (var file in Directory.GetFiles(UpdateTempFolder, " *.*", SearchOption.AllDirectories))
            {
                Log.Information($"Copying file {file}");
                if (file == "data.db")
                {
                    Log.Information("Skipping dababase file");
                    continue; ;
                }
                File.Copy(file, file.Replace($"{UpdateTempFolder}\\", ""), true);
            }
        }

        private void RemoveUpdateLeftovers()
        {
            Log.Information("---------------------------------");
            Log.Information("Removing update leftover");
            if (Directory.Exists($"{Directory.GetCurrentDirectory()}/{UpdateTempFolder}"))
            {
                Directory.Delete($"{Directory.GetCurrentDirectory()}/{UpdateTempFolder}", true);
            }

            if (File.Exists(_updateFileName))
            {
                File.Delete(_updateFileName);
            }
        }

        private bool KillProcessAndWait()
        {
            Log.Information("---------------------------------");
            Log.Information($"Stopping Embystat process (process id: {_processId})");
            var proc = Process.GetProcessById(_processId);

            if (proc.HasExited)
            {
                return true;
            }

            proc.Kill();
            proc.WaitForExit(20000);

            return proc.HasExited;
        }
    }
}
