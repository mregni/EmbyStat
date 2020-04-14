using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using EmbyStat.Common;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
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
        private static NLog.Logger _logger;
        public static int Main(string[] args)
        {
            try
            {
                StartupOptions options = null;
                var result = Parser.Default.ParseArguments<StartupOptions>(args).MapResult(opts => options = opts, NotParedOptions);

                if (options == null)
                {
                    return 0;
                }
                
                var configArgs = CreateArgsArray(options);
                _logger = SetupLogging(configArgs);
                LogLevelChanger.SetNlogLogLevel(NLog.LogLevel.FromOrdinal(options.LogLevel));

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
                _logger?.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tServer shutdown");
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        private static StartupOptions NotParedOptions(IEnumerable<Error> errs)
        {
            var errors = errs.ToList();
            if (errors.Any(x => x is HelpRequestedError))
            {
                Console.WriteLine("Help requested by user so application will not start.");
            }
            else if (errors.Any(x => x is VersionRequestedError))
            {
                Console.WriteLine("Version requested by user so application will not start.");
            }
            else
            {
                Console.WriteLine($"errors {errors.Count}");
                Console.WriteLine("Can't map startup settings, please check settings or use --help for more info.");
            }
            
            return null;
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

        private static NLog.Logger SetupLogging(Dictionary<string, string> configArgs)
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
            var dataPath = GetDataPath(options);
            return new Dictionary<string, string>
            {
                { "Port", options.Port.ToString() }, 
                { "NoUpdates", options.NoUpdates.ToString() },
                { "Dirs:Data", dataPath},
                { "Dirs:Config", GetConfigPath(options, dataPath) },
                { "Dirs:Logs", GetLogsPath(options, dataPath) },
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
                    dataDir = Directory.GetCurrentDirectory();
                }
            }

            try
            {
                Directory.CreateDirectory(dataDir);
            }
            catch (Exception e)
            {
                _logger.Warn("Can't create data directory:");
                _logger.Fatal(e);
            }

            return dataDir;
        }

        private static string GetConfigPath(StartupOptions options, string basePath)
        {
            var configDir = options.ConfigDir;
            if (string.IsNullOrWhiteSpace(configDir))
            {
                configDir = Environment.GetEnvironmentVariable("EMBYSTAT_CONFIG_DIR");
                if (string.IsNullOrWhiteSpace(configDir))
                {
                    configDir = basePath;
                }
            }

            try
            {
                Directory.CreateDirectory(configDir);
            }
            catch (Exception e)
            {
                _logger.Warn("Can't create config directory:");
                _logger.Fatal(e);
            }

            return configDir;
        }

        private static string GetLogsPath(StartupOptions options, string basePath)
        {
            var logDir = options.LogDir;
            if (string.IsNullOrWhiteSpace(logDir))
            {
                logDir = Environment.GetEnvironmentVariable("EMBYSTAT_LOG_DIR");
                if (string.IsNullOrWhiteSpace(logDir))
                {
                    logDir = Path.Combine(basePath, "Logs");
                }
            }

            try
            {
                Directory.CreateDirectory(logDir);
            }
            catch (Exception e)
            {
                _logger.Warn("Can't create log directory:");
                _logger.Fatal(e);
            }

            return logDir;
        }
    }
}

