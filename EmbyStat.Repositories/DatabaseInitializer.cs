using System;
using System.Threading.Tasks;
using EmbyStat.Repositories.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Repositories
{
	public class DatabaseInitializer : IDatabaseInitializer
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<DatabaseInitializer> _logger;

		public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task SeedAsync()
		{
			_logger.LogInformation("Migrating database started");
			_context.Database.Migrate();
			_logger.LogInformation("Migrating database ended");

			if (!await _context.Configuration.AnyAsync())
			{
				_logger.LogInformation("Initialising configuration");

				var config = new Configuration()
				{
					Id = Guid.NewGuid().ToString(),
					Language = "en",
				};

				_context.Configuration.Add(config);

				await _context.SaveChangesAsync();
			}

			//foreach (var syncronisation in _context.Syncronisations)
			//{
			// syncronisation.IsRunning = false;
			//}

			await _context.SaveChangesAsync();
		}
	}
}
