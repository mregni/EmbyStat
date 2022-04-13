using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Repositories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Constants = EmbyStat.Common.Constants;

// ReSharper disable All

namespace EmbyStat.Web;

public class Program
{
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
            SetupLogger(configArgs["Dirs:Logs"], options.LogLevel);
            LogStartupParameters(configArgs, options.LogLevel, options.Service);

            var listeningUrl = string.Join(';', options.ListeningUrls.Split(';').Select(x => $"{x}:{options.Port}"));
            var config = BuildConfigurationRoot(configArgs);

            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .CaptureStartupErrors(true)
                        .UseKestrel()
                        .UseStartup<Startup>()
                        .UseUrls(listeningUrl)
                        .UseConfiguration(config)
                        .ConfigureLogging(builder =>
                        {
                            builder.ClearProviders();
                            builder.SetMinimumLevel(LogLevel.Debug);
                            builder.AddSerilog();
                        });
                })
                .Build();

            SetupDatabase(host);

            host.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Log.Fatal(ex, "Server terminated unexpectedly");
            return 1;
        }
        finally
        {
            Console.WriteLine($"{Constants.LogPrefix.System}\tServer shutdown");
            Log.Information("Server shutdown");
            Log.CloseAndFlush();
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

    public static IConfigurationRoot BuildConfigurationRoot(Dictionary<string, string> configArgs) =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, false)
            .AddInMemoryCollection(configArgs)
            .Build();

    private static void SetupDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EsDbContext>();
        db.Database.Migrate();
    }

    private static void SetupLogger(string logPath, int logLevel)
    {
        var levelSwitch = new LoggingLevelSwitch();
        levelSwitch.MinimumLevel = logLevel == 1 ? LogEventLevel.Debug : LogEventLevel.Information;
        var minimumLevel = logLevel == 1 ? LogEventLevel.Debug : LogEventLevel.Warning;
        var logFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{SourceContext}]  [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        var fullLogPath = Path.Combine(logPath, "log.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console(outputTemplate: logFormat)
            .WriteTo.File(fullLogPath, rollingInterval: RollingInterval.Day, outputTemplate: logFormat)
            .MinimumLevel.Override("Hangfire.BackgroundJobServer", LogEventLevel.Warning) 
            .MinimumLevel.Override("Hangfire.Server.BackgroundServerProcess", LogEventLevel.Warning) 
            .MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Warning) 
            .MinimumLevel.Override("Hangfire.Processing.BackgroundExecution", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning) 
            .MinimumLevel.Override("Microsoft.AspNetCore", minimumLevel) 
            .MinimumLevel.Override("Microsoft.AspNetCore.SpaServices", LogEventLevel.Warning) 
            .MinimumLevel.Override("System.Net.Http.HttpClient", minimumLevel)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        
    }

    private static void LogStartupParameters(IReadOnlyDictionary<string, string> options, int logLevel, bool service)
    {
        var logLevelStr = logLevel == 1 ? "Debug" : "Information";
        var updatesEnabled = options["NoUpdates"] == "False";
        Log.Information("--------------------------------------------------------------------");
        Log.Information("System info:");
        Log.Information($"\tEnvironment\t{GetEnvironmentName()}");
        Log.Information($"\tDebugger\t{Debugger.IsAttached}");
        Log.Information($"\tProcess Name\t{GetProcessName()}");
        Log.Information($"\tLog level:\t{logLevelStr}");
        Log.Information($"\tPort:\t\t{options["Port"]}");
        Log.Information($"\tURL's:\t\t{options["ListeningUrls"]}");
        Log.Information($"\tConfigDir:\t{options["Dirs:Config"]}");
        Log.Information($"\tDataDir:\t{options["Dirs:Data"]}");
        Log.Information($"\tLogDir:\t\t{options["Dirs:Logs"]}");
        Log.Information($"\tCan update:\t{updatesEnabled}");
        Log.Information($"\tAs service:\t{service}");
        Log.Information("--------------------------------------------------------------------");
    }

    private static string GetProcessName()
    {
        using (var process = System.Diagnostics.Process.GetCurrentProcess())
        {
            return process.ProcessName;
        }
    }

    private static string GetEnvironmentName()
    {
        var str = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(str))
            str = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(str))
            str = "Production";
        return str;
    }

    private static Dictionary<string, string> CreateArgsArray(StartupOptions options)
    {
        var dataPath = GetDataPath(options);
        return new Dictionary<string, string>
        {
            {"Port", options.Port.ToString()},
            {"ListeningUrls", options.ListeningUrls},
            {"NoUpdates", options.NoUpdates.ToString()},
            {"Dirs:Data", dataPath},
            {"Dirs:Config", GetConfigPath(options, dataPath)},
            {"Dirs:Logs", GetLogsPath(options, dataPath)}
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
            Log.Fatal(e, "Can't create data directory:");
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
            Log.Fatal(e, "Can't create config directory:");
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
            Log.Fatal(e, "Can't create log directory:");
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