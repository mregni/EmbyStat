using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using EmbyStat.Common;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EmbyStat.Web
{
    public class Program
    {
        private static Logger _logger;
        public static int Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<StartupOptions>(args);
                StartupOptions options = null;
                result.WithParsed(opts => options = opts);

                var configArgs = CreateArgsArray(options);
                _logger = SetupLogging(configArgs);

                var listeningUrl = $"http://*:{options.Port}";
                _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tBooting up server on port {options.Port}");
                
                var config = BuildConfigurationRoot(configArgs);
                var host = BuildWebHost(args, listeningUrl, config);

                SetupDatabase(host);

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.Log(NLog.LogLevel.Fatal, ex, $"{Constants.LogPrefix.System}\tServer terminated unexpectedly");
                return 1;
            }
            finally
            {
                Console.WriteLine($"{Constants.LogPrefix.System}\tServer shutdown");
                _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tServer shutdown");
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args, string listeningUrl, IConfigurationRoot config) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls(listeningUrl)
                .UseConfiguration(config)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog()
                .Build();

        public static IConfigurationRoot BuildConfigurationRoot(Dictionary<string, string> configArgs) =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddInMemoryCollection(configArgs)
                .Build();

        private static Logger SetupLogging(Dictionary<string, string> configArgs)
        {
            var logPath = configArgs.Single(x => x.Key == "Dirs:Logs").Value;
            var configPath = configArgs.Single(x => x.Key == "Dirs:Config").Value;

            var destination = Path.Combine(configPath, "nlog.config");
            if (!File.Exists(Path.Combine(configPath, "nlog.config")))
            {
                var source = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");
                File.Copy(source, destination);
            }

            var logger = NLogBuilder.ConfigureNLog(destination).GetCurrentClassLogger();

            var target = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            target.FileName = $"{logPath}/${{shortdate}}.log";
            LogManager.ReconfigExistingLoggers();

            return logger;
        }

        private static void SetupDatabase(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var databaseInitializer = services.GetRequiredService<IDatabaseInitializer>();
                    databaseInitializer.CreateIndexes();
                    databaseInitializer.SeedAsync();
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex);
                    throw;
                }
            }
        }

        private static Dictionary<string, string> CreateArgsArray(StartupOptions options)
        {
            return new Dictionary<string, string>
            {
                { "Port", options.Port.ToString() }, 
                { "NoUpdates", options.NoUpdates.ToString() },
                { "Dirs:Data", GetDataPath(options) },
                { "Dirs:Config", GetConfigPath(options) },
                { "Dirs:Logs", GetLogsPath(options) },
            };
        }

        private static string GetDataPath(StartupOptions options)
        {
            var dataDir = options.DataDir;
            if (string.IsNullOrWhiteSpace(dataDir))
            {
                dataDir = Environment.GetEnvironmentVariable("EMBYSTAT_DATA_DIR");
                if (string.IsNullOrWhiteSpace(dataDir))
                {
                    dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EmbyStat");
                }
            }

            return dataDir;
        }

        private static string GetConfigPath(StartupOptions options)
        {
            var configDir = options.ConfigDir;
            if (string.IsNullOrWhiteSpace(configDir))
            {
                configDir = Environment.GetEnvironmentVariable("EMBYSTAT_CONFIG_DIR");
                if (string.IsNullOrWhiteSpace(configDir))
                {
                    configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EmbyStat");
                }
            }
            return configDir;
        }

        private static string GetLogsPath(StartupOptions options)
        {
            var logDir = options.LogDir;
            if (string.IsNullOrWhiteSpace(logDir))
            {
                logDir = Environment.GetEnvironmentVariable("EMBYSTAT_LOG_DIR");
                if (string.IsNullOrWhiteSpace(logDir))
                {
                    logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EmbyStat", "Logs");
                }
            }
            return logDir;
        }
    }
}

