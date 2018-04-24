using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EmbyStat.Repositories
{
	public class DatabaseInitializer : IDatabaseInitializer
	{
		private readonly ApplicationDbContext _context;

		public DatabaseInitializer(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task SeedAsync()
		{
			Log.Information("Migrating database started");
			_context.Database.Migrate();
			Log.Information("Migrating database ended");

			if (!await _context.Configuration.AnyAsync())
			{
				Log.Information("Initialising configuration");

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
