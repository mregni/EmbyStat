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
	                Language = "en-US",
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
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Dansk", Code = "da-DK" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Ελληνικά", Code = "el-GR" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Español", Code = "es-ES" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Suomi", Code = "fi-FI" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Français", Code = "fr-FR" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Magyar", Code = "hu-HU" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Italiano", Code = "it-IT" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Norsk", Code = "no-NO" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Polski", Code = "pl-PL" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Brasileiro", Code = "pt-BR" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Português", Code = "pt-PT" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Românesc", Code = "ro-RO" },
	            new Language { Id = Guid.NewGuid().ToString(), Name = "Svenska", Code = "sv-SE" }
            };

	        _context.Languages.AddRange(languages);
	        await _context.SaveChangesAsync();
        }
	}
}
