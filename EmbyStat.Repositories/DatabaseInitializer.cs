using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
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
            await SeedConfiguration();
            await SeedLanguages();
            await SeedEmbyStatus();
            await SeedJobs();

            await _context.SaveChangesAsync();
        }

        private async Task SeedConfiguration()
        {
            Log.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding configuration");

            var configuration = _context.Configuration.ToList();

            if (configuration.All(x => x.Id != Constants.Configuration.Language))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.Language, Value = "en-US" });
            if (configuration.All(x => x.Id != Constants.Configuration.ToShortMovie))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ToShortMovie, Value = "10" });
            if (configuration.All(x => x.Id != Constants.Configuration.WizardFinished))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.WizardFinished, Value = "False" });
            if (configuration.All(x => x.Id != Constants.Configuration.UserName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.UserName, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.EmbyServerAddress))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyServerAddress, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.EmbyUserId))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserId, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.ServerName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ServerName, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.EmbyUserName))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserName, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.AccessToken))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.AccessToken, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.LastTvdbUpdate))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.LastTvdbUpdate, Value = string.Empty });
            if (configuration.All(x => x.Id != Constants.Configuration.TvdbApiKey))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.TvdbApiKey, Value = "BWLRSNRC0AQUIEYX" });
            if (configuration.All(x => x.Id != Constants.Configuration.KeepLogsCount))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.KeepLogsCount, Value = "10" });
            if (configuration.All(x => x.Id != Constants.Configuration.MovieCollectionTypes))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.MovieCollectionTypes, Value = "[0, 1, 6]" });
            if (configuration.All(x => x.Id != Constants.Configuration.ShowCollectionTypes))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ShowCollectionTypes, Value = "[0, 2]" });
            if (configuration.All(x => x.Id != Constants.Configuration.EmbyServerPort))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyServerPort, Value = "8096" });
            if (configuration.All(x => x.Id != Constants.Configuration.EmbyServerProtocol))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyServerProtocol, Value = "0" });
            if (configuration.All(x => x.Id != Constants.Configuration.AutoUpdate))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.AutoUpdate, Value = "True" });
            if (configuration.All(x => x.Id != Constants.Configuration.UpdateTrain))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.UpdateTrain, Value = "1" });
            if (configuration.All(x => x.Id != Constants.Configuration.UpdateInProgress))
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.UpdateInProgress, Value = "False" });

            await _context.SaveChangesAsync();
        }

        private async Task SeedLanguages()
        {
            Log.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding languages");

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

        private async Task SeedEmbyStatus()
        {
            Log.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding Emby status");

            var status = _context.EmbyStatus.ToList();

            if (status.All(x => x.Id != Constants.EmbyStatus.MissedPings))
                _context.EmbyStatus.Add(new EmbyStatusKeyValue { Id = Constants.EmbyStatus.MissedPings, Value = "0" });

            await _context.SaveChangesAsync();
        }

        private async Task SeedJobs()
        {
            Log.Debug("${Constants.LogPrefix.DatabaseSeeder}\tSeeding job data");

            var jobs = _context.Jobs.ToList();

            if (!jobs.Exists(x => x.Id == Constants.JobIds.CheckUpdateId))
                _context.Jobs.Add(new Job { Id = Constants.JobIds.CheckUpdateId, State = JobState.Idle, Description = "CHECKUPDATEDESCRIPTION", Title = "CHECKUPDATETITLE", Trigger = "0 */12 * * *", CurrentProgressPercentage = 0, EndTimeUtc = null, StartTimeUtc = null });
            if (!jobs.Exists(x => x.Id == Constants.JobIds.SmallSyncId))
                _context.Jobs.Add(new Job { Id = Constants.JobIds.SmallSyncId, State = JobState.Idle, Description = "SMALLEMBYSYNCDESCRIPTION", Title = "SMALLEMBYSYNCTITLE", Trigger = "0 2 * * *", CurrentProgressPercentage = 0, EndTimeUtc = null, StartTimeUtc = null });
            if (!jobs.Exists(x => x.Id == Constants.JobIds.MediaSyncId))
                _context.Jobs.Add(new Job { Id = Constants.JobIds.MediaSyncId, State = JobState.Idle, Description = "MEDIASYNCDESCRIPTION", Title = "MEDIASYNCTITLE", Trigger = "0 3 * * *", CurrentProgressPercentage = 0, EndTimeUtc = null, StartTimeUtc = null });
            if (!jobs.Exists(x => x.Id == Constants.JobIds.PingEmbyId))
                _context.Jobs.Add(new Job { Id = Constants.JobIds.PingEmbyId, State = JobState.Idle, Description = "PINGEMBYSERVERDESCRIPTION", Title = "PINGEMBYSERVERTITLE", Trigger = "*/5 * * * *", CurrentProgressPercentage = 0, EndTimeUtc = null, StartTimeUtc = null });
            if (!jobs.Exists(x => x.Id == Constants.JobIds.DatabaseCleanupId))
                _context.Jobs.Add(new Job { Id = Constants.JobIds.DatabaseCleanupId, State = JobState.Idle, Description = "DATABASECLEANUPDESCRIPTION", Title = "DATABASECLEANUPTITLE", Trigger = "0 4 * * *", CurrentProgressPercentage = 0, EndTimeUtc = null, StartTimeUtc = null });

            await _context.SaveChangesAsync();
        }
    }
}
