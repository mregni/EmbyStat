using System;

namespace EmbyStat.Common
{
    public static class Constants
    {
        public static class Movies
        {
            public static string TotalMovies => "MOVIES.TOTALMOVIES";
            public static string TotalGenres => "MOVIES.TOTALGENRES";
            public static string LowestRated => "MOVIES.LOWESTRATED";
            public static string HighestRated => "MOVIES.HIGHESTRATED";
            public static string OldestPremiered => "MOVIES.OLDESTPREMIERED";
            public static string YoungestPremiered => "MOVIES.YOUNGESTPREMIERED";
            public static string Shortest => "MOVIES.SHORTEST";
            public static string Longest => "MOVIES.LONGEST";
            public static string YoungestAdded => "MOVIES.YOUNGESTADDED";
            public static string TotalPlayLength => "MOVIES.TOTALPLAYLENGTH";
        }

        public static class Common
        {
            public static string TotalActors => "COMMON.TOTALACTORS";
            public static string TotalDirectors => "COMMON.TOTALDIRECTORS";
            public static string TotalWriters => "COMMON.TOTALWRITERS";
            public static string MostFeaturedActor => "COMMON.MOSTFEATUREDACTOR";
            public static string MostFeaturedDirector => "COMMON.MOSTFEATUREDDIRECTOR";
            public static string MostFeaturedWriter => "COMMON.MOSTFEATUREDWRITER";
        }

        public static class Shows
        {
            public static string TotalShows => "SHOWS.TOTALSHOWS";
            public static string TotalEpisodes => "SHOWS.TOTALEPISODES";
            public static string TotalMissingEpisodes => "SHOWS.TOTALMISSINGEPISODES";
            public static string TotalPlayLength => "SHOWS.TOTALPLAYLENGTH";
            public static string HighestRatedShow => "SHOWS.HIGHESTRATEDSHOW";
            public static string LowestRatedShow => "SHOWS.LOWESTRATEDSHOW";
            public static string OldestPremiered => "SHOWS.OLDESTPREMIERED";
            public static string YoungestPremiered => "SHOWS.YOUNGESTPREMIERED";
            public static string YoungestAdded => "SHOWS.YOUNGESTADDED";
            public static string MostEpisodes => "SHOWS.WITHMOESTEPISODES";
            public static string ShowStatusGraph => "SHOWS.SHOWSTATUSGRAPH";
        }

        public static class Emby
        {
            public static string DeviceName => "EmbyStats server";
            public static string DeviceId => "6d5082f6-dffd-4ce9-8301-03eb339e05d4";
            public static string AppName => "EmbyStats";
            public static string AppVersion => "1.0.0";
            public static string AuthorizationScheme => "MediaBrowser";
        }

        public static class Tvdb
        {
            public static string BaseUrl => "https://api.thetvdb.com";
            public static string SerieEpisodesUrl => "/series/{0}/episodes?page=>{1}";
            public static string UpdatesUrl => "/updated/query?fromTime=>{0}&toTime=>{1}";
            public static string LoginUrl => "/login";
        }

        public static class Type
        {
            public static string Movie => "Movie";
            public static string Boxset => "BoxSet";
        }

        public static class Configuration
        {
            public static string AccessToken => "ACCESSTOKEN";
            public static string EmbyServerAddress => "EMBYSERVERADDRESS";
            public static string EmbyServerPort => "EMBYSERVERPORT";
            public static string EmbyServerProtocol => "EMBYSERVERPROTOCOL";
            public static string EmbyUserId => "EMBYUSERID";
            public static string EmbyUserName => "EMBYUSERNAME";
            public static string Language => "LANGUAGE";
            public static string LastTvdbUpdate => "LASTTVDBUPDATE";
            public static string ServerName => "SERVERNAME";
            public static string ToShortMovie => "TOSHORTMOVIE";
            public static string UserName => "USERNAME";
            public static string WizardFinished => "WIZARDFINISHED";
            public static string TvdbApiKey => "TVDBAPICLIENT";
            public static string KeepLogsCount => "KEEPLOGSCOUNT";
            public static string MovieCollectionTypes => "MOVIECOLLECTIONTYPES";
            public static string ShowCollectionTypes => "SHOWCOLLECTIONTYPES";
            public static string AutoUpdate => "AUTOUPDATE";
            public static string UpdateTrain => "UPDATETRAIN";
            public static string UpdateInProgress => "UPDATEINPROGRESS";
        }

        public static class EmbyStatus
        {
            public static string MissedPings => "MISSEDPINGS";
        }

        public static class LogPrefix
        {
            public static string ServerApi => "SERVER-API";
            public static string TaskManager => "TASK-MANAGER";
            public static string TaskWorker => "TASK-WORKER";
            public static string DatabaseSeeder => "DATABASE-SEEDER";
            public static string MediaSyncJob => "MEDIASYNC-JOB";
            public static string CheckUpdateJob => "CHECKUDPATE-JOB";
            public static string PingEmbyJob => "PINGEMBYSERVER-JOB";
            public static string SmallEmbySyncJob => "SMALLEMBYSYNC-JOB";
            public static string System => "SYSTEM";
            public static string EmbyClient => "EMBY-CLIENT";
            public static string TheTVDBCLient => "THETVDB-CLIENT";
            public static string ExceptionHandler => "EXCEPTION-HANDLER";
            public static string DatabaseCleanupJob => "DATABASE CLEANUP-JOB";
            public static string JobController => "JOB CONTROLLER";
        }

        public static class JobIds
        {
            public static Guid MediaSyncId => new Guid("be68900b-ee1d-41ef-b12f-60ef3106052e");
            public static Guid SmallSyncId => new Guid("41e0bf22-1e6b-4f5d-90be-ec966f746a2f");
            public static Guid CheckUpdateId => new Guid("78bc2bf0-abd9-48ef-aeff-9c396d644f2a");
            public static Guid PingEmbyId => new Guid("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e");
            public static Guid DatabaseCleanupId => new Guid("b109ca73-0563-4062-a3e2-f7e6a00b73e9");
        }

        //CHARTS
        public static string CountPerGenre => "COMMON.COUNTPERGENRE";
        public static string CountPerCommunityRating => "COMMON.COUNTPERCOMMUNITYRATING";
        public static string CountPerPremiereYear => "COMMON.COUNTPERPREMIEREDATE";
        public static string CountPerCollectedRate => "COMMON.COUNTPERCOLLECTEDRATE";
        public static string CountPerOfficialRating => "COMMON.COUNTPEROFFICIALRATING";

        //COMMON
        public static string Unknown => "UNKNOWN";

        //SERVER
        public static string TempFolder => "temp";

        //QUALITIES
        public static string FOURK => "4K";
        public static string THOUSANDFOURFOURP => "1440P";
        public static string FULLHD => "1080P";
        public static string HDREADY => "720P";
        public static string FOURHUNDERDEIGHTY => "480P";
        public static string LOWEST => "< 480P";

        //DATABASE
        public static string Actor => "Actor";
        public static string Director => "Director";
        public static string Writer => "Writer";

        //DUPLICATE REASONS
        public static string ByImdb => "BYIMDB";
        public static string ByTitle => "BYTITLE";

        public static class Tables
        {
            public static string BoxSets => "BoxSets";
            public static string Collections => "Collections";
            public static string Configuration => "Configuration";
            public static string Devices => "Devices";
            public static string Drives => "Drives";
            public static string EmbyStatus => "EmbyStatus";
            public static string Genres => "Genres";
            public static string Languages => "Languages";
            public static string Media => "Media";
            public static string People => "People";
            public static string Plugins => "Plugins";
            public static string ServerInfo => "ServerInfo";
            public static string Statistics => "Statistics";
            public static string Jobs => "Jobs";
            public static string User => "Users";
            public static string AudioStreams => "AudioStreams";
            public static string MediaCollection => "MediaCollection";
            public static string MediaGenres => "MediaGenres";
            public static string MediaSources => "MediaSources";
            public static string SeasonEpisodes => "SeasonEpisodes";
            public static string SubtitleStreams => "SubtitleStreams";
            public static string VideoStreams => "VideoStreams";
            public static string ExtraPersons => "ExtraPersons";
            public static string StatisticCollection => "StatisticCollection";
        }
    }
}
