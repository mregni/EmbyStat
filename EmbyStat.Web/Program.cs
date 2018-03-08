using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EmbyStat.Repositories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

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

#if !DEBUG
				OpenBrowser("http://localhost:5123");
#endif

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
				.UseStartup<Startup>()
				.UseSerilog()
				.Build();


		public static void CreateLogger()
		{
			Log.Logger = new LoggerConfiguration()
#if DEBUG
				.MinimumLevel.Debug()
#else
				.MinimumLevel.Information()
#endif
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
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

		public static void OpenBrowser(string url)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
		}
	}
}

