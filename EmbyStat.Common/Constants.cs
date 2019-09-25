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
            public static string NewestPremiered => "MOVIES.NEWESTPREMIERED";
            public static string Shortest => "MOVIES.SHORTEST";
            public static string Longest => "MOVIES.LONGEST";
            public static string LatestAdded => "MOVIES.LATESTADDED";
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
            public static string TotalDiskSize => "COMMON.TOTALDISKSIZE";
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
            public static string NewestPremiered => "SHOWS.NEWESTPREMIERED";
            public static string LatestAdded => "SHOWS.LATESTADDED";
            public static string MostEpisodes => "SHOWS.WITHMOESTEPISODES";
            public static string ShowStatusChart => "SHOWS.SHOWSTATUSGRAPH";
        }

        public static class Users
        {
            public static string TotalWatchedMovies => "USERS.STATS.TOTALWATCHEDMOVIES";
            public static string TotalWatchedEpisodes => "USERS.STATS.TOTALWATCHEDEPISODES";
        }

        public static class Tvdb
        {
            public static string BaseUrl => "https://api.thetvdb.com";
            public static string SerieEpisodesUrl => "/series/{0}/episodes?page={1}";
            public static string UpdatesUrl => "/updated/query?fromTime={0}&toTime={1}";
            public static string LoginUrl => "/login";
        }

        public static class Type
        {
            public static string Movie => "Movie";
            public static string Boxset => "BoxSet";
        }

        public static class LogPrefix
        {
            public static string ServerApi => "SERVER-API";
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
        
        //QUALITIES
        public static string FOURK => "4K";
        public static string THOUSANDFOURFOURP => "1440P";
        public static string FULLHD => "1080P";
        public static string HDREADY => "720P";
        public static string FOURHUNDERDEIGHTY => "480P";
        public static string LOWEST => "< 480P";

        //DUPLICATE REASONS
        public static string ByImdb => "BYIMDB";

        public static class PersonType
        {
            public static string Actor = "Actor";
            public static string Director = "Director";
            public static string Writer = "Writer";
        }
    }
}
