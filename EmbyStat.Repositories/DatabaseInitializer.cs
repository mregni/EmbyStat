using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
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
	        Log.Information("Seeding configuration");

	        var configuration = _context.Configuration.ToList();

            if(configuration.All(x => x.Id != Constants.Configuration.Language))
	            _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.Language, Value = "en-US" });
	        if(configuration.All(x => x.Id != Constants.Configuration.ToShortMovie))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ToShortMovie, Value = "10" });
	        if(configuration.All(x => x.Id != Constants.Configuration.WizardFinished))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.WizardFinished, Value = "False" });
	        if(configuration.All(x => x.Id != Constants.Configuration.UserName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.UserName, Value = string.Empty });
	        if(configuration.All(x => x.Id != Constants.Configuration.EmbyServerAddress))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyServerAddress, Value = string.Empty });
	        if(configuration.All(x => x.Id != Constants.Configuration.EmbyUserId))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserId, Value = string.Empty });
	        if(configuration.All(x => x.Id != Constants.Configuration.ServerName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ServerName, Value = string.Empty });
            if(configuration.All(x => x.Id != Constants.Configuration.EmbyUserName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserName, Value = string.Empty });
	        if(configuration.All(x => x.Id != Constants.Configuration.AccessToken))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.AccessToken, Value = string.Empty });
	        if (configuration.All(x => x.Id != Constants.Configuration.LastTvdbUpdate))
	            _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.LastTvdbUpdate, Value = string.Empty });
	        if (configuration.All(x => x.Id != Constants.Configuration.TvdbApiKey))
	            _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.TvdbApiKey, Value = "BWLRSNRC0AQUIEYX" });
	        if (configuration.All(x => x.Id != Constants.Configuration.KeepLogsCount))
	            _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.KeepLogsCount, Value = "10" });

            await _context.SaveChangesAsync();
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
