
using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Services.System
{
	public class SystemService : ISystemService
	{
		private readonly IApplicationLifetime _applicationLifetime;
		private readonly ILogger<SystemService> _logger;

		public SystemService(IApplicationLifetime applicationLifetime, ILogger<SystemService> logger)
		{
			_applicationLifetime = applicationLifetime;
			_logger = logger;
		}

		public void StartShutdownJob()
		{
			BackgroundJob.Enqueue(() => ShutdownJob());
		}

		public void ShutdownJob()
		{
			_logger.LogInformation("Shutdown task started");
			_applicationLifetime.StopApplication();
		}
	}
}
