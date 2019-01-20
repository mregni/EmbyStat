using System;
using System.IO;
using CommandLine;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Updater.Models;

namespace Updater
{
    class Program
    {
        public static void Main(string[] args)
        {
            SetupLogger();

            Console.WriteLine(" ======================================");
            Console.WriteLine("        Starting EmbyStat Updater");
            Console.WriteLine(" ======================================");

            var result = Parser.Default.ParseArguments<StartupOptions>(args);
            StartupOptions options = null;
            result.WithParsed(opts => options = opts);
            Console.WriteLine("Fetching options");

            var updater = new Updater(options);
            updater.Start();
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
