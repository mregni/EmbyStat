using System;
using System.IO;
using CommandLine;
using NLog.Web;
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

            NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }
    }
}
