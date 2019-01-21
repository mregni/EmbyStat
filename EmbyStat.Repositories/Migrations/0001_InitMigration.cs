using System.Data;
using EmbyStat.Common;
using EmbyStat.Common.Models.Tasks.Enum;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(1)]
    public class InitMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Constants.Tables.BoxSets)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Boxsets")
                .WithColumn("ParentId").AsString().NotNullable()
                .WithColumn("Name").AsString(200).NotNullable()
                .WithColumn("OfficialRating").AsString().Nullable()
                .WithColumn("Primary").AsString().Nullable();

            Create.Table(Constants.Tables.Collections)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Collections")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("PrimaryImage").AsString().Nullable()
                .WithColumn("Type").AsInt32().NotNullable();

            Create.Table(Constants.Tables.Configuration)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Configuration")
                .WithColumn("Value").AsString().Nullable();

            Create.Table(Constants.Tables.Devices)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Devices")
                .WithColumn("AppName").AsString().Nullable()
                .WithColumn("AppVersion").AsString().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("LastUserName").AsString().Nullable();

            Create.Table(Constants.Tables.Drives)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Drives")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Path").AsString().NotNullable()
                .WithColumn("Type").AsString().NotNullable();

            Create.Table(Constants.Tables.EmbyStatus)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_EmbyStatus")
                .WithColumn("Value").AsString().NotNullable();

            Create.Table(Constants.Tables.Genres)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Genres")
                .WithColumn("Name").AsString().NotNullable();

            Create.Table(Constants.Tables.Languages)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Languages")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Code").AsString().NotNullable();

            Create.Table(Constants.Tables.Media)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Media")
                .WithColumn("DateCreated").AsDateTime().Nullable()
                .WithColumn("ParentId").AsGuid().NotNullable()
                .WithColumn("PremiereDate").AsDateTime().Nullable()
                .WithColumn("ProductionYear").AsInt32().Nullable()
                .WithColumn("Banner").AsString().Nullable()
                .WithColumn("Logo").AsString().Nullable()
                .WithColumn("Primary").AsString().Nullable()
                .WithColumn("Thumb").AsString().Nullable()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("Path").AsString().Nullable()
                .WithColumn("SortName").AsString().Nullable()
                .WithColumn("Discriminator").AsString().NotNullable()
                .WithColumn("CommunityRating").AsFloat().Nullable()
                .WithColumn("RunTimeTicks").AsInt64().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("IMDB").AsString().Nullable()
                .WithColumn("TMDB").AsString().Nullable()
                .WithColumn("TVDB").AsString().Nullable()
                .WithColumn("Container").AsString().Nullable()
                .WithColumn("HasSubtitles").AsBoolean().Nullable()
                .WithColumn("IdHD").AsBoolean().Nullable()
                .WithColumn("MediaType").AsString().Nullable()
                .WithColumn("DvdEpisodeNumber").AsFloat().Nullable()
                .WithColumn("DvdSeasonNumber").AsInt32().Nullable()
                .WithColumn("IndexNumber").AsInt32().Nullable()
                .WithColumn("IndexNumberEnd").AsInt32().Nullable()
                .WithColumn("HomePageUrl").AsString().Nullable()
                .WithColumn("OfficialRating").AsString().Nullable()
                .WithColumn("OriginalTitle").AsString().Nullable()
                .WithColumn("CumulativeRunTimeTicks").AsInt64().Nullable()
                .WithColumn("DateLastMediaAdded").AsDateTime().Nullable()
                .WithColumn("Status").AsString().Nullable()
                .WithColumn("TvdbSynced").AsBoolean().Nullable()
                .WithColumn("MissingEpisodesCount").AsInt32().Nullable()
                .WithColumn("TvdbFailed").AsString().Nullable()
                .WithColumn("Season_IndexNumber").AsInt32().Nullable()
                .WithColumn("Season_IndexNumberEnd").AsInt32().Nullable();

            Create.Table(Constants.Tables.People)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_People")
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("Synced").AsBoolean().NotNullable()
                .WithColumn("Etag").AsString().Nullable()
                .WithColumn("HomePageUrl").AsString().Nullable()
                .WithColumn("MovieCount").AsInt32().NotNullable()
                .WithColumn("OverView").AsString().Nullable()
                .WithColumn("BirthDate").AsDateTime().Nullable()
                .WithColumn("IMDB").AsString().Nullable()
                .WithColumn("TMDB").AsString().Nullable()
                .WithColumn("SeriesCount").AsInt32().NotNullable()
                .WithColumn("SortName").AsString().Nullable()
                .WithColumn("Primary").AsString().Nullable();

            Create.Table(Constants.Tables.Plugins)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Plugins")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Version").AsString().NotNullable()
                .WithColumn("ConfigurationFileName").AsString().Nullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("ImageTag").AsString().Nullable();

            Create.Table(Constants.Tables.ServerInfo)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_ServerInfo")
                .WithColumn("SystemUpdateLevel").AsInt16().NotNullable()
                .WithColumn("OperatingSystemDisplayName").AsString().NotNullable()
                .WithColumn("HasPendingRestart").AsBoolean().NotNullable()
                .WithColumn("IsShuttingDown").AsBoolean().NotNullable()
                .WithColumn("SupportsLibraryMonitor").AsBoolean().NotNullable()
                .WithColumn("WebSocketPortNumber").AsInt32().NotNullable()
                .WithColumn("CanSelfRestart").AsBoolean().NotNullable()
                .WithColumn("CanSelfUpdate").AsBoolean().NotNullable()
                .WithColumn("CanLaunchWebBrowser").AsBoolean().NotNullable()
                .WithColumn("ProgramDataPath").AsString().NotNullable()
                .WithColumn("ItemsByNamePath").AsString().NotNullable()
                .WithColumn("CachePath").AsString().NotNullable()
                .WithColumn("LogPath").AsString().NotNullable()
                .WithColumn("InternalMetadataPath").AsString().NotNullable()
                .WithColumn("TranscodingTempPath").AsString().NotNullable()
                .WithColumn("HttpServerPortNumber").AsInt32().NotNullable()
                .WithColumn("SupportsHttps").AsBoolean().NotNullable()
                .WithColumn("HttpsPortNumber").AsBoolean().NotNullable()
                .WithColumn("HasUpdateAvailable").AsBoolean().NotNullable()
                .WithColumn("SupportsAutoRunAtStartup").AsBoolean().NotNullable()
                .WithColumn("HardwareAccelerationRequiresPremiere").AsBoolean().NotNullable()
                .WithColumn("LocalAddress").AsString().NotNullable()
                .WithColumn("WanAddress").AsString().NotNullable()
                .WithColumn("ServerName").AsString().NotNullable()
                .WithColumn("Version").AsString().NotNullable()
                .WithColumn("OperatingSystem").AsString().NotNullable();

            Create.Table(Constants.Tables.Statistics)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Statistics")
                .WithColumn("CalculationDateTime").AsDateTime().NotNullable()
                .WithColumn("Type").AsInt32().NotNullable()
                .WithColumn("IsValid").AsBoolean().NotNullable()
                .WithColumn("JsonResult").AsString().NotNullable();

            Create.Table(Constants.Tables.Jobs)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Jobs")
                .WithColumn("StartTimeUtc").AsDateTime().Nullable()
                .WithColumn("EndTimeUtc").AsDateTime().Nullable()
                .WithColumn("State").AsInt32().NotNullable().WithDefaultValue(JobState.Idle)
                .WithColumn("CurrentProgressPercentage").AsDouble().NotNullable().WithDefaultValue(0)
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("Trigger").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable();

            Create.Table(Constants.Tables.User)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_User")
                .WithColumn("LastActivityDate").AsDateTime().Nullable()
                .WithColumn("LastLoginDate").AsDateTime().Nullable()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("IsAdmin").AsBoolean().NotNullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable()
                .WithColumn("IsDisabled").AsBoolean().NotNullable();

            Create.Table(Constants.Tables.AudioStreams)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_AudioStreams")
                .WithColumn("BitRate").AsInt64().Nullable()
                .WithColumn("ChannelLayout").AsString().Nullable()
                .WithColumn("Channels").AsInt32().NotNullable()
                .WithColumn("Codec").AsString().NotNullable()
                .WithColumn("Language").AsString().Nullable()
                .WithColumn("SampleRate").AsInt32().NotNullable()
                .WithColumn("VideoId").AsString().NotNullable()
                    .ForeignKey("FK_AudioStreams_Media_VideoId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.MediaCollection)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_MediaCollection")
                .WithColumn("MediaId").AsGuid().NotNullable()
                    .ForeignKey("FK_MediaCollection_Media_MediaId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade)
                .WithColumn("CollectionId").AsString().NotNullable()
                    .ForeignKey("FK_MediaCollection_Collections_CollectionId", Constants.Tables.Collections, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.MediaGenres)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_MediaGenres")
                .WithColumn("MediaId").AsGuid().NotNullable()
                    .ForeignKey("FK_MediaGenres_Media_MediaId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade)
                .WithColumn("GenreId").AsGuid().NotNullable()
                    .ForeignKey("FK_MediaGenres_Genres_GenreId", Constants.Tables.Genres, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.MediaSources)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_MediaSources")
                .WithColumn("BitRate").AsInt64().Nullable()
                .WithColumn("Container").AsString().Nullable()
                .WithColumn("Path").AsString().Nullable()
                .WithColumn("Protocol").AsString().Nullable()
                .WithColumn("RunTimeTicks").AsInt64().Nullable()
                .WithColumn("VideoType").AsString().Nullable()
                .WithColumn("VideoId").AsString().NotNullable()
                    .ForeignKey("FK_MediaSources_Media_VideoId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.SeasonEpisodes)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_SeasonEpisodes")
                .WithColumn("SeasonId").AsString().NotNullable()
                    .ForeignKey("FK_SeasonEpisodes_Media_EpisodeId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade)
                .WithColumn("EpisodeId").AsString().NotNullable()
                    .ForeignKey("FK_SeasonEpisodes_Media_SeasonId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.SubtitleStreams)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_SubtitleStreams")
                .WithColumn("Codec").AsString().Nullable()
                .WithColumn("DisplayTitle").AsString().Nullable()
                .WithColumn("IsDefault").AsBoolean().NotNullable()
                .WithColumn("Language").AsString().Nullable()
                .WithColumn("VideoId").AsString().NotNullable()
                    .ForeignKey("FK_SubtitleStreams_Media_VideoId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.VideoStreams)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_VideoStreams")
                .WithColumn("AspectRatio").AsString().Nullable()
                .WithColumn("AverageFrameRate").AsString().Nullable()
                .WithColumn("BitRate").AsInt64().Nullable()
                .WithColumn("Channels").AsInt32().Nullable()
                .WithColumn("Height").AsInt32().Nullable()
                .WithColumn("Language").AsString().Nullable()
                .WithColumn("Width").AsInt32().Nullable()
                .WithColumn("VideoId").AsString().NotNullable()
                .ForeignKey("FK_VideoStreams_Media_VideoId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.ExtraPersons)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_ExtraPersons")
                .WithColumn("Type").AsString().Nullable()
                .WithColumn("ExtraId").AsString().NotNullable()
                .ForeignKey("FK_ExtraPersons_Media_ExtraId", Constants.Tables.Media, "Id").OnDelete(Rule.Cascade)
                .WithColumn("PersonId").AsString().NotNullable()
                .ForeignKey("FK_ExtraPersons_People_PersonId", Constants.Tables.People, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.StatisticCollection)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_StatisticCollection")
                .WithColumn("StatisticId").AsGuid().NotNullable()
                .ForeignKey("FK_StatisticCollection_Statistics_StatisticId", Constants.Tables.Statistics, "Id").OnDelete(Rule.Cascade)
                .WithColumn("CollectionId").AsString().NotNullable()
                .ForeignKey("FK_StatisticCollection_Collections_CollectionId", Constants.Tables.Collections, "Id").OnDelete(Rule.Cascade);

            Create.Index("IX_AudioStreams_VideoId").OnTable(Constants.Tables.AudioStreams).OnColumn("VideoId");
            Create.Index("IX_ExtraPersons_PersonId").OnTable(Constants.Tables.ExtraPersons).OnColumn("PersonId");
            Create.Index("IX_MediaCollection_CollectionId").OnTable(Constants.Tables.MediaCollection).OnColumn("CollectionId");
            Create.Index("IX_MediaCollection_MediaId").OnTable(Constants.Tables.MediaCollection).OnColumn("MediaId");
            Create.Index("IX_MediaGenres_MediaId").OnTable(Constants.Tables.MediaGenres).OnColumn("MediaId");
            Create.Index("IX_MediaSources_VideoId").OnTable(Constants.Tables.MediaSources).OnColumn("VideoId");
            Create.Index("IX_SeasonEpisodes_SeasonId").OnTable(Constants.Tables.SeasonEpisodes).OnColumn("SeasonId");
            Create.Index("IX_StatisticCollection_CollectionId").OnTable(Constants.Tables.StatisticCollection).OnColumn("CollectionId");
            Create.Index("IX_StatisticCollection_StatisticId").OnTable(Constants.Tables.StatisticCollection).OnColumn("StatisticId");
            Create.Index("IX_SubtitleStreams_VideoId").OnTable(Constants.Tables.SubtitleStreams).OnColumn("VideoId");
            Create.Index("IX_VideoStreams_VideoId").OnTable(Constants.Tables.VideoStreams).OnColumn("VideoId");
            Create.Index("IX_ExtraPersons_ExtraId").OnTable(Constants.Tables.ExtraPersons).OnColumn("ExtraId");
            Create.Index("IX_SeasonEpisodes_EpisodeId").OnTable(Constants.Tables.SeasonEpisodes).OnColumn("EpisodeId");
        }
    }
}
