using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using Xunit;

namespace Tests.Unit.Repository
{
    public class DatabaseInitializerTests : BaseRepositoryTester
    {
        private DbContext _context;
        private DatabaseInitializer _databaseInitializer;
        public DatabaseInitializerTests() : base("test-data-db-init.db")
        {
        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _databaseInitializer = new DatabaseInitializer(_context);
        }

        #region Seed

        [Fact]
        public void SeedAsync_Should_Seed_All_Languages()
        {
            RunTest(() =>
            {
                _databaseInitializer.SeedAsync();
                using var context = _context.LiteDatabase;
                var collection = context.GetCollection<Language>();
                var languages = collection.FindAll().ToList();

                languages.Count.Should().Be(17);
                languages.Count(x => x.Name == "Nederlands").Should().Be(1);
                languages.Count(x => x.Name == "English").Should().Be(1);
                languages.Count(x => x.Name == "Deutsch").Should().Be(1);
                languages.Count(x => x.Name == "Dansk").Should().Be(1);
                languages.Count(x => x.Name == "Ελληνικά").Should().Be(1);
                languages.Count(x => x.Name == "Español").Should().Be(1);
                languages.Count(x => x.Name == "Suomi").Should().Be(1);
                languages.Count(x => x.Name == "Français").Should().Be(1);
                languages.Count(x => x.Name == "Magyar").Should().Be(1);
                languages.Count(x => x.Name == "Italiano").Should().Be(1);
                languages.Count(x => x.Name == "Norsk").Should().Be(1);
                languages.Count(x => x.Name == "Polski").Should().Be(1);
                languages.Count(x => x.Name == "Brasileiro").Should().Be(1);
                languages.Count(x => x.Name == "Português").Should().Be(1);
                languages.Count(x => x.Name == "Românesc").Should().Be(1);
                languages.Count(x => x.Name == "Svenska").Should().Be(1);
                languages.Count(x => x.Name == "简体中文").Should().Be(1);

                languages.Count(x => x.Code == "nl-NL").Should().Be(1);
                languages.Count(x => x.Code == "en-US").Should().Be(1);
                languages.Count(x => x.Code == "de-DE").Should().Be(1);
                languages.Count(x => x.Code == "da-DK").Should().Be(1);
                languages.Count(x => x.Code == "el-GR").Should().Be(1);
                languages.Count(x => x.Code == "es-ES").Should().Be(1);
                languages.Count(x => x.Code == "fi-FI").Should().Be(1);
                languages.Count(x => x.Code == "fr-FR").Should().Be(1);
                languages.Count(x => x.Code == "hu-HU").Should().Be(1);
                languages.Count(x => x.Code == "it-IT").Should().Be(1);
                languages.Count(x => x.Code == "no-NO").Should().Be(1);
                languages.Count(x => x.Code == "pl-PL").Should().Be(1);
                languages.Count(x => x.Code == "pt-BR").Should().Be(1);
                languages.Count(x => x.Code == "pt-PT").Should().Be(1);
                languages.Count(x => x.Code == "ro-RO").Should().Be(1);
                languages.Count(x => x.Code == "sv-SE").Should().Be(1);
                languages.Count(x => x.Code == "cs-CZ").Should().Be(1);
            });
        }

        [Fact]
        public void SeedAsync_Should_Seed_MediaServerStatus()
        {
            RunTest(() =>
            {
                _databaseInitializer.SeedAsync();
                using var context = _context.LiteDatabase;
                var collection = context.GetCollection<MediaServerStatus>();
                var status = collection.FindAll().FirstOrDefault();

                status.Should().NotBeNull();
                // ReSharper disable once PossibleNullReferenceException
                status.MissedPings = 0;
            });
        }

        [Fact]
        public void SeedAsync_Should_Seed_Jobs()
        {
            RunTest(() =>
            {
                _databaseInitializer.SeedAsync();
                using var context = _context.LiteDatabase;
                var collection = context.GetCollection<Job>();
                var jobs = collection.FindAll().ToList();

                jobs.Count.Should().Be(6);
                jobs.Count(x => x.Id == Constants.JobIds.CheckUpdateId).Should().Be(1);
                jobs.Count(x => x.Id == Constants.JobIds.SmallSyncId).Should().Be(1);
                jobs.Count(x => x.Id == Constants.JobIds.MovieSyncId).Should().Be(1);
                jobs.Count(x => x.Id == Constants.JobIds.ShowSyncId).Should().Be(1);
                jobs.Count(x => x.Id == Constants.JobIds.PingEmbyId).Should().Be(1);
                jobs.Count(x => x.Id == Constants.JobIds.DatabaseCleanupId).Should().Be(1);
            });
        }

        #endregion

        #region Indexes

        [Fact]
        public void CreateIndexes_Should_Setup_Library_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var collectionCollection = context.GetCollection<Library>();
                collectionCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                collectionCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                collectionCollection.EnsureIndex(x => x.Type).Should().BeTrue();
                collectionCollection.EnsureIndex(x => x.Primary).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Device_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var deviceCollection = context.GetCollection<Device>();
                deviceCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                deviceCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.AppName).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.AppVersion).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.DateLastActivity).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.Deleted).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.IconUrl).Should().BeTrue();
                deviceCollection.EnsureIndex(x => x.LastUserId).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_EmbyStatus_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var embyStatusCollection = context.GetCollection<MediaServerStatus>();
                embyStatusCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                embyStatusCollection.EnsureIndex(x => x.MissedPings).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_EmbyUser_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var embyUserCollection = context.GetCollection<EmbyUser>();
                embyUserCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                embyUserCollection.EnsureIndex(x => x.IsAdministrator).Should().BeTrue();
                embyUserCollection.EnsureIndex(x => x.Deleted).Should().BeTrue();
                embyUserCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                embyUserCollection.EnsureIndex(x => x.ServerId).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Episode_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var episodeCollection = context.GetCollection<Episode>();
                episodeCollection.EnsureIndex(x => x.Id).Should().BeFalse();
                episodeCollection.EnsureIndex(x => x.ShowId).Should().BeFalse();
                episodeCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.IndexNumber).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.DvdEpisodeNumber).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.IndexNumberEnd).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.LocationType).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.ShowName).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.DvdSeasonNumber).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.AudioStreams).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.CollectionId).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Banner).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.CommunityRating).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Container).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.DateCreated).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Genres).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.IMDB).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.TVDB).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.LastUpdated).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Logo).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.MediaSources).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.MediaType).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.OfficialRating).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.ParentId).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Path).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.People).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.PremiereDate).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.ProductionYear).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.RunTimeTicks).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Primary).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.SortName).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.TMDB).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.SubtitleStreams).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Thumb).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.Video3DFormat).Should().BeTrue();
                episodeCollection.EnsureIndex(x => x.VideoStreams).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Filters_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var filtersCollection = context.GetCollection<FilterValues>();
                filtersCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Genre_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var genreCollection = context.GetCollection<Genre>();
                genreCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                genreCollection.EnsureIndex(x => x.Name).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Job_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var jobsCollection = context.GetCollection<Job>();
                jobsCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                jobsCollection.EnsureIndex(x => x.Description).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.CurrentProgressPercentage).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.EndTimeUtc).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.LogName).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.StartTimeUtc).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.State).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.Title).Should().BeTrue();
                jobsCollection.EnsureIndex(x => x.Trigger).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Language_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var languageCollection = context.GetCollection<Language>();
                languageCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                languageCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                languageCollection.EnsureIndex(x => x.Code).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Play_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var playCollection = context.GetCollection<Play>();
                playCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                playCollection.EnsureIndex(x => x.UserId).Should().BeFalse();
                playCollection.EnsureIndex(x => x.Type).Should().BeTrue();
                playCollection.EnsureIndex(x => x.ParentId).Should().BeTrue();
                playCollection.EnsureIndex(x => x.AudioChannelLayout).Should().BeTrue();
                playCollection.EnsureIndex(x => x.AudioCodec).Should().BeTrue();
                playCollection.EnsureIndex(x => x.AudioLanguage).Should().BeTrue();
                playCollection.EnsureIndex(x => x.MediaId).Should().BeTrue();
                playCollection.EnsureIndex(x => x.PlayStates).Should().BeTrue();
                playCollection.EnsureIndex(x => x.SessionId).Should().BeTrue();
                playCollection.EnsureIndex(x => x.SubtitleCodec).Should().BeTrue();
                playCollection.EnsureIndex(x => x.SubtitleDisplayTitle).Should().BeTrue();
                playCollection.EnsureIndex(x => x.SubtitleLanguage).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoAspectRatio).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoAverageFrameRate).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoCodec).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoHeight).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoRealFrameRate).Should().BeTrue();
                playCollection.EnsureIndex(x => x.VideoWidth).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Movie_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var movieCollection = context.GetCollection<Movie>();
                movieCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.CollectionId).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.RunTimeTicks).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.SortName).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.IMDB).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.Primary).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.CommunityRating).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                movieCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.Banner).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.DateCreated).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.Genres).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.LastUpdated).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.Logo).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.OfficialRating).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.ParentId).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.Path).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.People).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.PremiereDate).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.ProductionYear).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.TMDB).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.Thumb).Should().BeTrue();
                movieCollection.EnsureIndex(x => x.OriginalTitle).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Person_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var personCollection = context.GetCollection<Person>();
                personCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                personCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                personCollection.EnsureIndex(x => x.SortName).Should().BeTrue();
                personCollection.EnsureIndex(x => x.Primary).Should().BeTrue();
                personCollection.EnsureIndex(x => x.BirthDate).Should().BeTrue();
                personCollection.EnsureIndex(x => x.Etag).Should().BeTrue();
                personCollection.EnsureIndex(x => x.HomePageUrl).Should().BeTrue();
                personCollection.EnsureIndex(x => x.IMDB).Should().BeTrue();
                personCollection.EnsureIndex(x => x.MovieCount).Should().BeTrue();
                personCollection.EnsureIndex(x => x.OverView).Should().BeTrue();
                personCollection.EnsureIndex(x => x.ShowCount).Should().BeTrue();
                personCollection.EnsureIndex(x => x.TMDB).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_PluginInfo_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var pluginInfoCollection = context.GetCollection<PluginInfo>();
                pluginInfoCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                pluginInfoCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                pluginInfoCollection.EnsureIndex(x => x.ConfigurationFileName).Should().BeTrue();
                pluginInfoCollection.EnsureIndex(x => x.Description).Should().BeTrue();
                pluginInfoCollection.EnsureIndex(x => x.ImageTag).Should().BeTrue();
                pluginInfoCollection.EnsureIndex(x => x.Version).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Season_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var seasonCollection = context.GetCollection<Season>();
                seasonCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                seasonCollection.EnsureIndex(x => x.ParentId).Should().BeFalse();
                seasonCollection.EnsureIndex(x => x.IndexNumberEnd).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.IndexNumber).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.LocationType).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.DateCreated).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.Banner).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.CollectionId).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.Logo).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.Path).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.PremiereDate).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.Primary).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.ProductionYear).Should().BeTrue();
                seasonCollection.EnsureIndex(x => x.SortName).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_ServerInfo_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var serverInfoCollection = context.GetCollection<ServerInfo>();
                serverInfoCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                serverInfoCollection.EnsureIndex(x => x.CachePath).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.CanLaunchWebBrowser).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.CanSelfRestart).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.CanSelfUpdate).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.HasPendingRestart).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.HasUpdateAvailable).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.HttpServerPortNumber).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.HttpsPortNumber).Should().BeTrue();
                serverInfoCollection.EnsureIndex(x => x.InternalMetadataPath).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Session_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var sessionCollection = context.GetCollection<Session>();
                sessionCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                sessionCollection.EnsureIndex(x => x.UserId).Should().BeFalse();
                sessionCollection.EnsureIndex(x => x.Client).Should().BeTrue();
                sessionCollection.EnsureIndex(x => x.AppIconUrl).Should().BeTrue();
                sessionCollection.EnsureIndex(x => x.ApplicationVersion).Should().BeTrue();
                sessionCollection.EnsureIndex(x => x.DeviceId).Should().BeTrue();
                sessionCollection.EnsureIndex(x => x.Plays).Should().BeTrue();
                sessionCollection.EnsureIndex(x => x.ServerId).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Show_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var showCollection = context.GetCollection<Show>();
                showCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                showCollection.EnsureIndex(x => x.CumulativeRunTimeTicks).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Episodes).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Seasons).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Status).Should().BeTrue();
                showCollection.EnsureIndex(x => x.ExternalSyncFailed).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Name).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Banner).Should().BeTrue();
                showCollection.EnsureIndex(x => x.CollectionId).Should().BeTrue();
                showCollection.EnsureIndex(x => x.CommunityRating).Should().BeTrue();
                showCollection.EnsureIndex(x => x.DateCreated).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Genres).Should().BeTrue();
                showCollection.EnsureIndex(x => x.IMDB).Should().BeTrue();
                showCollection.EnsureIndex(x => x.LastUpdated).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Logo).Should().BeTrue();
                showCollection.EnsureIndex(x => x.OfficialRating).Should().BeTrue();
                showCollection.EnsureIndex(x => x.ParentId).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Path).Should().BeTrue();
                showCollection.EnsureIndex(x => x.People).Should().BeTrue();
                showCollection.EnsureIndex(x => x.PremiereDate).Should().BeTrue();
                showCollection.EnsureIndex(x => x.ProductionYear).Should().BeTrue();
                showCollection.EnsureIndex(x => x.RunTimeTicks).Should().BeTrue();
                showCollection.EnsureIndex(x => x.SortName).Should().BeTrue();
                showCollection.EnsureIndex(x => x.TMDB).Should().BeTrue();
                showCollection.EnsureIndex(x => x.Thumb).Should().BeTrue();
            });
        }

        [Fact]
        public void CreateIndexes_Should_Setup_Statistic_Indexes()
        {
            RunTest(() =>
            {
                _databaseInitializer.CreateIndexes();
                using var context = _context.LiteDatabase;
                var statisticsCollection = context.GetCollection<Statistic>();
                statisticsCollection.EnsureIndex(x => x.Id, true).Should().BeFalse();
                statisticsCollection.EnsureIndex(x => x.Type).Should().BeTrue();
                statisticsCollection.EnsureIndex(x => x.IsValid).Should().BeTrue();
                statisticsCollection.EnsureIndex(x => x.CalculationDateTime).Should().BeTrue();
                statisticsCollection.EnsureIndex(x => x.CollectionIds).Should().BeTrue();
                statisticsCollection.EnsureIndex(x => x.JsonResult).Should().BeTrue();
            });
        }

        #endregion
    }
}
