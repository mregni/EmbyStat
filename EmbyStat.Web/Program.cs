using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using EmbyStat.Common;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Repositories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog;
using NLog.Targets;
using DbContext = EmbyStat.Repositories.DbContext;
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
                var parseResult = Parser.Default.ParseArguments<StartupOptions>(args)
                    .WithParsed(startupOptions =>
                    {
                        if (startupOptions.LogLevel == 1)
                        {
                            Console.WriteLine("------------------------------");
                            Console.WriteLine("!Application is in Debug mode!");
                            Console.WriteLine("------------------------------");
                        }
                    })
                    ;

                if (parseResult.Tag == ParserResultType.NotParsed)
                {
                    return 0;
                }

                StartupOptions options = null;
                parseResult.MapResult(opt => options = opt, NotParedOptions);

                options = CheckEnvironmentVariables(options);

                var configArgs = CreateArgsArray(options);
                _logger = SetupLogging(configArgs);
                LogLevelChanger.SetNlogLogLevel(NLog.LogLevel.FromOrdinal(options.LogLevel));

                LogStartupParameters(configArgs, options.LogLevel, options.Service);

                var listeningUrl = string.Join(';', options.ListeningUrls.Split(';').Select(x => $"{x}:{options.Port}"));
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
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DbContext>();
            db.Database.Migrate();
        }

        private static void LogStartupParameters(IReadOnlyDictionary<string, string> options, int logLevel, bool service)
        {
            var logLevelStr = logLevel == 1 ? "Debug" : "Information";
            var updatesEnabled = options["NoUpdates"] == "False";
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t--------------------------------------------------------------------");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tBooting up server with following options:");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tLog level:\t\t{logLevelStr}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tPort:\t\t\t{options["Port"]}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tURL's:\t\t\t{options["ListeningUrls"]}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tConfigDir:\t\t{options["Dirs:Config"]}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tDataDir:\t\t{options["Dirs:Data"]}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tLogDir:\t\t\t{options["Dirs:Logs"]}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tUpdates enabled:\t{updatesEnabled}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t\tRunning as service:\t{service}");
            _logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\t--------------------------------------------------------------------");
        }

        private static Dictionary<string, string> CreateArgsArray(StartupOptions options)
        {
            var dataPath = GetDataPath(options);
            return new Dictionary<string, string>
            {
                { "Port", options.Port.ToString() },
                { "ListeningUrls", options.ListeningUrls },
                { "NoUpdates", options.NoUpdates.ToString() },
                { "Dirs:Data", dataPath},
                { "Dirs:Config", GetConfigPath(options, dataPath) },
                { "Dirs:Logs", GetLogsPath(options, dataPath) }
            };
        }

        private static string GetDataPath(StartupOptions options)
        {
            var dataDir = options.DataDir;
            if (string.IsNullOrWhiteSpace(dataDir))
            {
                dataDir = Directory.GetCurrentDirectory();
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
                configDir = basePath;
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
                logDir = Path.Combine(basePath, "logs");
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

        private static StartupOptions CheckEnvironmentVariables(StartupOptions options)
        {
            var portStr = Environment.GetEnvironmentVariable("EMBYSTAT_PORT");
            if (portStr != null && int.TryParse(portStr, out var port))
            {
                options.Port = port;
            }

            var dataDir = Environment.GetEnvironmentVariable("EMBYSTAT_DATADIR");
            if (dataDir != null)
            {
                options.DataDir = dataDir;
            }

            var configDir = Environment.GetEnvironmentVariable("EMBYSTAT_CONFIGDIR");
            if (configDir != null)
            {
                options.ConfigDir = configDir;
            }

            var logDir = Environment.GetEnvironmentVariable("EMBYSTAT_LOGDIR");
            if (logDir != null)
            {
                options.LogDir = logDir;
            }

            var logLevelStr = Environment.GetEnvironmentVariable("EMBYSTAT_LOGLEVEL");
            if (logLevelStr != null && int.TryParse(logLevelStr, out var logLevel))
            {
                options.LogLevel = logLevel;
            }

            var listeningUrls = Environment.GetEnvironmentVariable("EMBYSTAT_LISTENURL");
            if (listeningUrls != null)
            {
                options.ListeningUrls = listeningUrls;
            }

            var noUpdatesStr = Environment.GetEnvironmentVariable("EMBYSTAT_NOUPDATES");
            if (noUpdatesStr != null && bool.TryParse(noUpdatesStr, out var noUpdates))
            {
                options.Service = noUpdates;
            }

            var serviceStr = Environment.GetEnvironmentVariable("EMBYSTAT_SERVICE");
            if (serviceStr != null && bool.TryParse(serviceStr, out var service))
            {
                options.Service = service;
            }

            return options;
        }
    }
}

