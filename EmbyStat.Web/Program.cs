using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CommandLine;
using EmbyStat.Common.Configuration;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Generators;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using YamlDotNet.Serialization.NamingConventions;
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

            var mode = GetApplicationMode();
            switch (mode)
            {
                case ApplicationModes.Interactive:
                    var builder = BuildConsoleHost(options);
                    builder.Run();
                    break;
                default:
                    break;
            }
         
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
    
    private static ApplicationModes GetApplicationMode()
    {
        // if (OperatingSystem.IsWindows() && startupContext.RegisterUrl)
        // {
        //     return ApplicationModes.RegisterUrl;
        // }
        //
        // if (OperatingSystem.IsWindows() && startupContext.InstallService)
        // {
        //     return ApplicationModes.InstallService;
        // }
        //
        // if (OperatingSystem.IsWindows() && startupContext.UninstallService)
        // {
        //     return ApplicationModes.UninstallService;
        // }

        // IsWindowsService can throw sometimes, so wrap it
        var isWindowsService = false;
        // try
        // {
        //     isWindowsService = WindowsServiceHelpers.IsWindowsService();
        // }
        // catch (Exception e)
        // {
        //     Logger.LogError(e, "Failed to get service status");
        // }

        if (OperatingSystem.IsWindows() && isWindowsService)
        {
            return ApplicationModes.Service;
        }

        return ApplicationModes.Interactive;
    }

    private static IHost BuildConsoleHost(StartupOptions options)
    {
        var memoryConfig = options.ToKeyValuePairs();
        
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Add<WritableJsonConfigurationSource>(
                (Action<WritableJsonConfigurationSource>)(s =>
                {
                    s.FileProvider = null;
                    s.Path = Path.Combine("config", "config.json");
                    s.Optional = false;
                    s.ReloadOnChange = true;
                    s.ResolveFileProvider();
                }))
            .AddInMemoryCollection(memoryConfig)
            .Build();
        
        SetupLogger(config["Dirs:Logs"], options.LogLevel ?? 2);
        LogStartupParameters(config, options.LogLevel ?? 2, options.RunAsService ?? false);

        if (string.IsNullOrWhiteSpace(config["Jwt:Key"]))
        {
            Log.Logger.Debug("Generating JWT key");
            config["Jwt:Key"] = KeyGenerator.GetUniqueKey(120);
        }
        
        var bindAddress = config["Hosting:Url"];
        var port = Convert.ToInt32(config["Hosting:Port"]);
        var sslPort = Convert.ToInt32(config["Hosting:SslPort"]);
        var sslEnalbed = Convert.ToBoolean(config["Hosting:SslEnalbed"]);
        var sslCertPath = config["Hosting:SslCertPath"];
        var sslCertPassword = config["Hosting:SslCertPassword"];

        var urls = new List<string> { BuildUrl("http", bindAddress, port) };

        if (sslEnalbed && !string.IsNullOrWhiteSpace(sslCertPath))
        {
            urls.Add(BuildUrl("https", bindAddress, sslPort));
        }
        
        return Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .CaptureStartupErrors(true)
                    .UseKestrel(options =>
                    {
                        if (sslEnalbed && !string.IsNullOrWhiteSpace(sslCertPath))
                        {
                            options.ConfigureHttpsDefaults(configureOptions =>
                            {
                                configureOptions.ServerCertificate = ValidateSslCertificate(sslCertPath, sslCertPassword);
                            });
                        }
                    })
                    .UseStartup<Startup>()
                    .UseUrls(urls.ToArray())
                    .UseConfiguration(config)
                    .ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AllowSynchronousIO = true;
                        serverOptions.Limits.MaxRequestBodySize = null;
                    })
                    .ConfigureLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.SetMinimumLevel(LogLevel.Debug);
                        builder.AddSerilog();
                    });
            })
            .Build();
    }
    
    private static X509Certificate2 ValidateSslCertificate(string cert, string password)
    {
        X509Certificate2 certificate;

        try
        {
            certificate = new X509Certificate2(cert, password, X509KeyStorageFlags.DefaultKeySet);
        }
        catch (CryptographicException ex)
        {
            if (ex.HResult == 0x2 || ex.HResult == 0x2006D080)
            {
                throw new EmbyStatStartupException(ex, $"The SSL certificate file {cert} does not exist");
            }

            throw new EmbyStatStartupException(ex);
        }

        return certificate;
    }
    
    private static string BuildUrl(string scheme, string bindAddress, int port)
    {
        return $"{scheme}://{bindAddress}:{port}";
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
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Model", LogEventLevel.Warning) 
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Connection", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.ChangeTracking", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", minimumLevel) 
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", minimumLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore.SpaServices", LogEventLevel.Warning) 
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning) 
            .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles", LogEventLevel.Warning) 
            .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.SignalR", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Microsoft.AspNetCore.Mvc.Infrastructure", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Cors", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.JwtBearer", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", minimumLevel)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        
    }

    private static void LogStartupParameters(IConfigurationRoot config, int logLevel, bool service)
    {
        var logLevelStr = logLevel == 1 ? "Debug" : "Information";
        Log.Information("--------------------------------------------------------------------");
        Log.Information("System info:");
        Log.Information($"\tEnvironment\t{GetEnvironmentName()}");
        Log.Information($"\tDebugger\t{Debugger.IsAttached}");
        Log.Information($"\tProcess Name\t{GetProcessName()}");
        Log.Information($"\tVersion\t\t{Assembly.GetExecutingAssembly().GetName().Version}");
        Log.Information($"\tLog level:\t{logLevelStr}");
        Log.Information($"\tPort:\t\t{config["Hosting:Port"]}");
        Log.Information($"\tSSL Port:\t{config["Hosting:SslPort"]}");
        Log.Information($"\tSSL Enabled:\t{config["Hosting:SslEnabled"]}");
        Log.Information($"\tURLs:\t\t{config["Hosting:Urls"]}");
        Log.Information($"\tConfig dir:\t{config["Dirs:Config"]}");
        Log.Information($"\tLog dir:\t{config["Dirs:Logs"]}");
        Log.Information($"\tData dir:\t{config["Dirs:Data"]}");
        Log.Information($"\tCan update:\t{config["UpdatesDisabled"]}");
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
            options.NoUpdates = noUpdates;
        }

        var serviceStr = Environment.GetEnvironmentVariable("EMBYSTAT_SERVICE");
        if (serviceStr != null && bool.TryParse(serviceStr, out var service))
        {
            options.RunAsService = service;
        }

        return options;
    }
}