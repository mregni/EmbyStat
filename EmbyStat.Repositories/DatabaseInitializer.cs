using System;
using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using NLog;
using Logger = NLog.Logger;

namespace EmbyStat.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbContext _context;
        private readonly Logger _logger;

        public DatabaseInitializer(IDbContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void CreateIndexes()
        {
            var collectionCollection = _context.GetContext().GetCollection<Library>();
            collectionCollection.EnsureIndex(x => x.Id, true);

            var deviceCollection = _context.GetContext().GetCollection<Device>();
            deviceCollection.EnsureIndex(x => x.Id, true);

            var embyStatusCollection = _context.GetContext().GetCollection<EmbyStatus>();
            embyStatusCollection.EnsureIndex(x => x.Id, true);

            var embyUserCollection = _context.GetContext().GetCollection<EmbyUser>();
            embyUserCollection.EnsureIndex(x => x.Id, true);

            var episodeCollection = _context.GetContext().GetCollection<Episode>();
            episodeCollection.EnsureIndex(x => x.Id);
            episodeCollection.EnsureIndex(x => x.ShowId);

            var genreCollection = _context.GetContext().GetCollection<Genre>();
            genreCollection.EnsureIndex(x => x.Id, true);

            var jobsCollection = _context.GetContext().GetCollection<Job>();
            jobsCollection.EnsureIndex(x => x.Id, true);

            var languageCollection = _context.GetContext().GetCollection<Language>();
            languageCollection.EnsureIndex(x => x.Id, true);

            var playCollection = _context.GetContext().GetCollection<Play>();
            playCollection.EnsureIndex(x => x.Id, true);
            playCollection.EnsureIndex(x => x.UserId);

            var movieCollection = _context.GetContext().GetCollection<Movie>();
            movieCollection.EnsureIndex(x => x.Id, true);
            movieCollection.EnsureIndex(x => x.CollectionId);
            movieCollection.EnsureIndex(x => x.RunTimeTicks);
            movieCollection.EnsureIndex(x => x.SortName);
            movieCollection.EnsureIndex(x => x.IMDB);
            movieCollection.EnsureIndex(x => x.Primary);
            movieCollection.EnsureIndex(x => x.CommunityRating);

            var personCollection = _context.GetContext().GetCollection<Person>();
            personCollection.EnsureIndex(x => x.Id, true);

            var pluginInfoCollection = _context.GetContext().GetCollection<PluginInfo>();
            pluginInfoCollection.EnsureIndex(x => x.Id, true);

            var seasonCollection = _context.GetContext().GetCollection<Season>();
            seasonCollection.EnsureIndex(x => x.Id, true);
            seasonCollection.EnsureIndex(x => x.ParentId);

            var serverInfoCollection = _context.GetContext().GetCollection<ServerInfo>();
            serverInfoCollection.EnsureIndex(x => x.Id, true);

            var sessionCollection = _context.GetContext().GetCollection<Session>();
            sessionCollection.EnsureIndex(x => x.Id, true);
            sessionCollection.EnsureIndex(x => x.UserId);

            var showCollection = _context.GetContext().GetCollection<Show>();
            showCollection.EnsureIndex(x => x.Id, true);

            var statisticsCollection = _context.GetContext().GetCollection<Statistic>();
            statisticsCollection.EnsureIndex(x => x.Id, true);
        }

        public void SeedAsync()
        {
            SeedLanguages();
            SeedEmbyStatus();
            SeedJobs();
        }

        private void SeedLanguages()
        {
            _logger.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding languages");
            var collection = _context.GetContext().GetCollection<Language>();

            if (!collection.Exists(Query.All()))
            {
                var languages = new List<Language>
                {
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Nederlands", Code = "nl-NL"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "English", Code = "en-US"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Deutsch", Code = "de-DE"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Dansk", Code = "da-DK"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Ελληνικά", Code = "el-GR"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Español", Code = "es-ES"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Suomi", Code = "fi-FI"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Français", Code = "fr-FR"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Magyar", Code = "hu-HU"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Italiano", Code = "it-IT"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Norsk", Code = "no-NO"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Polski", Code = "pl-PL"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Brasileiro", Code = "pt-BR"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Português", Code = "pt-PT"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Românesc", Code = "ro-RO"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "Svenska", Code = "sv-SE"},
                    new Language {Id = Guid.NewGuid().ToString(), Name = "简体中文", Code = "cs-CZ"}
                };

                collection.InsertBulk(languages);
            }
        }

        private void SeedEmbyStatus()
        {
            _logger.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding MediaServer status");
            var collection = _context.GetContext().GetCollection<EmbyStatus>();

            if (!collection.Exists(Query.All()))
            {
                var status = new EmbyStatus { MissedPings = 0 };
                collection.Insert(status);
            }
        }

        private void SeedJobs()
        {
            _logger.Debug($"{Constants.LogPrefix.DatabaseSeeder}\tSeeding job data");
            var collection = _context.GetContext().GetCollection<Job>();

            if (!collection.Exists(Query.All()))
            {
                var jobs = new List<Job>
                {
                    new Job
                    {
                        Id = Constants.JobIds.CheckUpdateId,
                        State = JobState.Idle,
                        Description = "CHECKUPDATEDESCRIPTION",
                        Title = "CHECKUPDATETITLE",
                        Trigger = "0 */12 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.SmallSyncId,
                        State = JobState.Idle,
                        Description = "SMALLEMBYSYNCDESCRIPTION",
                        Title = "SMALLEMBYSYNCTITLE",
                        Trigger = "0 2 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.MediaSyncId,
                        State = JobState.Idle,
                        Description = "MEDIASYNCDESCRIPTION",
                        Title = "MEDIASYNCTITLE",
                        Trigger = "0 3 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.PingEmbyId,
                        State = JobState.Idle,
                        Description = "PINGEMBYSERVERDESCRIPTION",
                        Title = "PINGEMBYSERVERTITLE",
                        Trigger = "*/5 * * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.DatabaseCleanupId,
                        State = JobState.Idle,
                        Description = "DATABASECLEANUPDESCRIPTION",
                        Title = "DATABASECLEANUPTITLE",
                        Trigger = "0 4 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    }
                };

                collection.InsertBulk(jobs);
            }
        }
    }
}
