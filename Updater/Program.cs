using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace Updater
{
    class Program
    {
        public static void Main(string[] args)
        {
            SetupLogger();

            Log.Information("-------------------------------------------------");
            Log.Information("|            Starting update process            |");
            Log.Information("-------------------------------------------------");

            foreach (var arg in args)
            {
                Log.Information(arg);
            }

            var failing = false;
            var processIdConverted = int.TryParse(args[0], out var processId);
            if (args[0] == null || !processIdConverted)
            {
                Log.Warning("Halting updater, no procces id passed to updater!");
                failing = true;
            }

            if (string.IsNullOrWhiteSpace(args[1]))
            {
                Log.Warning("Halting updater, no update url passed to updater!");
                failing = true;
            }

            if (string.IsNullOrWhiteSpace(args[2]))
            {
                Log.Warning("Halting updater, no update filename passed to updater!");
                failing = true;
            }

            if (!failing)
            {
                var updater = new Updater(args[1], args[2], processId);
                failing = updater.Start();

            }

            RestartingEmbyStat();

            if (!failing)
            {
                Log.Information("Exiting updater process");
                Log.Information("Enjoy your new version!");
            }
            else
            {
                Log.Warning("Update failed!");
            }
        }

        private static void SetupLogger()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithRootName("Exception"))
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/updater.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        private static void RestartingEmbyStat()
        {
            Log.Information("Restarting EmbyStat");
            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "EmbyStat.Web.dll",
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory()
            });

            Log.Information("EmbyStat process started");
        }
    }
}
