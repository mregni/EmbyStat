using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CommandLine;
using EmbyStat.Common;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Repositories.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rollbar;
using Rollbar.PlugIns.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace EmbyStat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateLogger();

                var result = Parser.Default.ParseArguments<StartupOptions>(args);
                StartupOptions options = null;
                result.WithParsed(opts => options = opts);

                var listeningUrl = $"http://*:{options.Port}";

                Log.Information($"{Constants.LogPrefix.System}\tBooting up server on port {options.Port}");
                var configArgs = new Dictionary<string, string> { { "Port", options.Port.ToString() } };
                var config = BuildConfigurationRoot(configArgs);
                var host = BuildWebHost(args, listeningUrl, config);

                SetupDatabase(host);
                CheckForUserSettingsFile();

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Fatal(ex, $"{Constants.LogPrefix.System}\tServer terminated unexpectedly");
            }
            finally
            {
                Console.WriteLine($"{Constants.LogPrefix.System}\tServer shutdown");
                Log.Information($"{Constants.LogPrefix.System}\tServer shutdown");
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args, string listeningUrl, IConfigurationRoot config) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseUrls(listeningUrl)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();

        public static IConfigurationRoot BuildConfigurationRoot(Dictionary<string, string> configArgs) =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine("Settings", "appsettings.json"), false, false)
                .AddInMemoryCollection(configArgs)
                .AddEnvironmentVariables()
                .Build();

        public static void CreateLogger()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
				.MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithRootName("Exception"))
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine("Logs", "log.txt"), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Sink(new RollbarSink(new RollbarConfig("204e4b6617394a33bdde354094490b04") { LogLevel = ErrorLevel.Error }, null, new CultureInfo("en-US")))
                .CreateLogger();
        }

        private static void SetupDatabase(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    UpdateDatabase();
                    var databaseInitializer = services.GetRequiredService<IDatabaseInitializer>();
                    databaseInitializer.SeedAsync().Wait();
                }
                catch (Exception ex)
                {
                    Log.Fatal($"{Constants.LogPrefix.System}\tDatabase seed or update failed");
                    Log.Fatal($"{Constants.LogPrefix.System}\t{ex.Message}\n{ex.StackTrace}");
                    throw;
                }
            }
        }

        private static void UpdateDatabase()
        {
            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString("Data Source=data.db")
                    .ScanIn(typeof(InitMigration).Assembly).For.Migrations())
                .BuildServiceProvider(false);

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private static void CheckForUserSettingsFile()
        {
            if (!File.Exists(Path.Combine("Settings", "usersettings.json")))
            {
                var e = new FileNotFoundException("usersettings.json file not found in Settings folder. Exiting program now!"); ;
                Log.Error(e, "Can't start server!");
                throw e;
            }
        }
    }
}

