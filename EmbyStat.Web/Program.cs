using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using EmbyStat.Common;
using EmbyStat.Repositories.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Fluent;
using NLog.Web;
using NLog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EmbyStat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = SetupLogging();

            try
            {
                var result = Parser.Default.ParseArguments<StartupOptions>(args);
                StartupOptions options = null;
                result.WithParsed(opts => options = opts);

                var listeningUrl = $"http://*:{options.Port}";

                logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tBooting up server on port {options.Port}");
                var configArgs = new Dictionary<string, string> { { "Port", options.Port.ToString() } };
                var config = BuildConfigurationRoot(configArgs);
                var host = BuildWebHost(args, listeningUrl, config);

                SetupDatabase(host);

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Log(NLog.LogLevel.Fatal, ex, $"{Constants.LogPrefix.System}\tServer terminated unexpectedly");
            }
            finally
            {
                Console.WriteLine($"{Constants.LogPrefix.System}\tServer shutdown");
                logger.Log(NLog.LogLevel.Info, $"{Constants.LogPrefix.System}\tServer shutdown");
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args, string listeningUrl, IConfigurationRoot config) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseUrls(listeningUrl)
                .UseConfiguration(config)
                .UseStartup<Startup>()
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

        private static Logger SetupLogging()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "config", "nlog.config")))
            {
                var source = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");
                var destination = Path.Combine(Directory.GetCurrentDirectory(), "config", "nlog.config");
                File.Move(source, destination);
            }

            var logger = NLogBuilder.ConfigureNLog(Path.Combine(Directory.GetCurrentDirectory(), "config", "nlog.config")).GetCurrentClassLogger();
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
                    Log.Fatal($"{Constants.LogPrefix.System}\tDatabase seed or update failed");
                    Log.Fatal($"{Constants.LogPrefix.System}\t{ex.Message}\n{ex.StackTrace}");
                    throw;
                }
            }
        }
    }
}

