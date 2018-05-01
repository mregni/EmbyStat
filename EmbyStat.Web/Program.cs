using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using EmbyStat.Repositories.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

				Log.Information("Booting up server");
				var host = BuildWebHost(args);

				SetupDatbase(host);

				host.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Server terminated unexpectedly");
			}
			finally
			{
				Log.Information("Server shutdown");
				Log.CloseAndFlush();
			}
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
				.UseSerilog()
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
				.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();
		}

		public static void SetupDatbase(IWebHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var databaseInitializer = services.GetRequiredService<IDatabaseInitializer>();
					databaseInitializer.SeedAsync().Wait();
				}
				catch (Exception ex)
				{
					Log.Fatal("Database seed or update failed");
					Log.Fatal($"{ex.Message}\n{ex.StackTrace}");
				}
			}
		}
	}
}

