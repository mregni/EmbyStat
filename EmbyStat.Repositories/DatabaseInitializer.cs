using System;
using System.Collections.Generic;
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

		    await SeedConfiguration();
		    await SeedLanguages();

			//foreach (var syncronisation in _context.Syncronisations)
			//{
			// syncronisation.IsRunning = false;
			//}

			await _context.SaveChangesAsync();
		}

	    private async Task SeedConfiguration()
	    {
	        if (!await _context.Configuration.AnyAsync())
	        {
	            Log.Information("Seeding configuration");

	            var config = new Configuration()
	            {
	                Id = Guid.NewGuid().ToString(),
	                Language = "en",
	                ToShortMovie = 10
	            };

	            _context.Configuration.Add(config);
	            await _context.SaveChangesAsync();
	        }
        }

	    private async Task SeedLanguages()
	    {
	        Log.Information("Seeding languages");

            _context.Languages.RemoveRange(_context.Languages);

	        var languages = new List<Language>
	        {
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Nederlands", Code = "nl-NL" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "English", Code = "en-US" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Deutsche", Code = "de-DE" },
	        };

	        _context.Languages.AddRange(languages);
	        await _context.SaveChangesAsync();
        }
	}
}
