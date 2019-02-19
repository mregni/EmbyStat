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
            await SeedLanguages();
            await SeedEmbyStatus();
            await SeedJobs();

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
                new Language { Id = Guid.NewGuid().ToString(), Name = "Svenska", Code = "sv-SE" },
                new Language { Id = Guid.NewGuid().ToString(), Name = "Chinees", Code = "cs-CZ"}
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
            Log.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding job data");

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
