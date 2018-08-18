using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupLogger();
            Log.Information("Starting update process");

            Log.Information("Update process finished, restarting EmbyStat");
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
    }
}
