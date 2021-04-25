using System;
using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Logging;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbContext _context;
        private readonly Logger _logger;

        public DatabaseInitializer(IDbContext context)
        {
            _context = context;
            _logger = LogFactory.CreateLoggerForType(typeof(DatabaseInitializer), "DATABASE-SEEDER");
        }

        public void CreateIndexes()
        {
            using var context = _context.CreateDatabaseContext();
            var collectionCollection = context.GetCollection<Library>();
            collectionCollection.EnsureIndex(x => x.Id, true);

            var deviceCollection = context.GetCollection<Device>();
            deviceCollection.EnsureIndex(x => x.Id, true);

            var embyStatusCollection = context.GetCollection<EmbyStatus>();
            embyStatusCollection.EnsureIndex(x => x.Id, true);

            var embyUserCollection = context.GetCollection<EmbyUser>();
            embyUserCollection.EnsureIndex(x => x.Id, true);

            var episodeCollection = context.GetCollection<Episode>();
            episodeCollection.EnsureIndex(x => x.Id);
            episodeCollection.EnsureIndex(x => x.ShowId);

            var filtersCollection = context.GetCollection<FilterValues>("Filters");
            filtersCollection.EnsureIndex(x => x.Id, true);

            var genreCollection = context.GetCollection<Genre>();
            genreCollection.EnsureIndex(x => x.Id, true);

            var jobsCollection = context.GetCollection<Job>();
            jobsCollection.EnsureIndex(x => x.Id, true);

            var languageCollection = context.GetCollection<Language>();
            languageCollection.EnsureIndex(x => x.Id, true);

            var playCollection = context.GetCollection<Play>();
            playCollection.EnsureIndex(x => x.Id, true);
            playCollection.EnsureIndex(x => x.UserId);

            var movieCollection = context.GetCollection<Movie>();
            movieCollection.EnsureIndex(x => x.Id, true);
            movieCollection.EnsureIndex(x => x.CollectionId);
            movieCollection.EnsureIndex(x => x.RunTimeTicks);
            movieCollection.EnsureIndex(x => x.SortName);
            movieCollection.EnsureIndex(x => x.IMDB);
            movieCollection.EnsureIndex(x => x.Primary);
            movieCollection.EnsureIndex(x => x.CommunityRating);

            var personCollection = context.GetCollection<Person>();
            personCollection.EnsureIndex(x => x.Id, true);

            var pluginInfoCollection = context.GetCollection<PluginInfo>();
            pluginInfoCollection.EnsureIndex(x => x.Id, true);

            var seasonCollection = context.GetCollection<Season>();
            seasonCollection.EnsureIndex(x => x.Id, true);
            seasonCollection.EnsureIndex(x => x.ParentId);

            var serverInfoCollection = context.GetCollection<ServerInfo>();
            serverInfoCollection.EnsureIndex(x => x.Id, true);

            var sessionCollection = context.GetCollection<Session>();
            sessionCollection.EnsureIndex(x => x.Id, true);
            sessionCollection.EnsureIndex(x => x.UserId);

            var showCollection = context.GetCollection<Show>();
            showCollection.EnsureIndex(x => x.Id, true);

            var statisticsCollection = context.GetCollection<Statistic>();
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
            _logger.Debug("Seeding languages");
            using var context = _context.CreateDatabaseContext();
            var collection = context.GetCollection<Language>();

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
            using var context = _context.CreateDatabaseContext();
            _logger.Debug("Seeding MediaServer status");
            var collection = context.GetCollection<EmbyStatus>();

            if (!collection.Exists(Query.All()))
            {
                var status = new EmbyStatus { MissedPings = 0 };
                collection.Insert(status);
            }
        }

        private void SeedJobs()
        {
            using var context = _context.CreateDatabaseContext();
            _logger.Debug("Seeding job data");
            var collection = context.GetCollection<Job>();

            if (!collection.Exists(Query.All()))
            {
                var jobs = new List<Job>
                {
                    new Job
                    {
                        Id = Constants.JobIds.CheckUpdateId,
                        State = JobState.Idle,
                        Description = $"{Constants.LogPrefix.CheckUpdateJob}DESCRIPTION",
                        Title = $"{Constants.LogPrefix.CheckUpdateJob}",
                        Trigger = "0 */12 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.SmallSyncId,
                        State = JobState.Idle,
                        Description = $"{Constants.LogPrefix.SmallMediaServerSyncJob}DESCRIPTION",
                        Title = $"{Constants.LogPrefix.SmallMediaServerSyncJob}",
                        Trigger = "0 2 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.MediaSyncId,
                        State = JobState.Idle,
                        Description = $"{Constants.LogPrefix.MediaSyncJob}DESCRIPTION",
                        Title = $"{Constants.LogPrefix.MediaSyncJob}",
                        Trigger = "0 3 * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.PingEmbyId,
                        State = JobState.Idle,
                        Description = $"{Constants.LogPrefix.PingMediaServerJob}DESCRIPTION",
                        Title = $"{Constants.LogPrefix.PingMediaServerJob}",
                        Trigger = "*/5 * * * *",
                        CurrentProgressPercentage = 0,
                        EndTimeUtc = null,
                        StartTimeUtc = null
                    },
                    new Job
                    {
                        Id = Constants.JobIds.DatabaseCleanupId,
                        State = JobState.Idle,
                        Description = $"{Constants.LogPrefix.DatabaseCleanupJob}DESCRIPTION",
                        Title = $"{Constants.LogPrefix.DatabaseCleanupJob}",
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
