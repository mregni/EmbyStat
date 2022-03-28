using System;

namespace EmbyStat.Common
{
    public static class Constants
    {
        public static class Tables
        {
            public static string Movies => "Movies";
            public static string MediaSources => "MediaSources";
            public static string VideoStreams => "VideoStreams";
            public static string AudioStreams => "AudioStreams";
            public static string SubtitleStreams => "SubtitleStreams";
            public static string GenreMovie => "SqlGenreSqlMovie";
            public static string MediaPerson => "MediaPerson";
            public static string People => "People";
            public static string Genres => "Genres";
            public static string Shows => "Shows";
            public static string Seasons => "Seasons";
            public static string Episodes => "Episodes";
            public static string GenreShow => "SqlGenreSqlShow";
            public static string Libraries => "Libraries";
        }
        public static class Movies
        {
            public static string TotalMovies => "MOVIES.TOTALMOVIES";
            public static string LowestRated => "MOVIES.LOWESTRATED";
            public static string HighestRated => "MOVIES.HIGHESTRATED";
            public static string OldestPremiered => "MOVIES.OLDESTPREMIERED";
            public static string NewestPremiered => "MOVIES.NEWESTPREMIERED";
            public static string Shortest => "MOVIES.SHORTEST";
            public static string Longest => "MOVIES.LONGEST";
            public static string LatestAdded => "MOVIES.LATESTADDED";
            public static string TotalPlayLength => "MOVIES.TOTALPLAYLENGTH";
        }

        public static class Icons
        {
            public static string TheatersRoundedIcon => "TheatersRoundedIcon";
            public static string StorageRoundedIcon => "StorageRoundedIcon";
            public static string QueryBuilderRoundedIcon => "QueryBuilderRoundedIcon";
            public static string PoundRoundedIcon => "PoundRoundedIcon";
            public static string PeopleAltRoundedIcon => "PeopleAltRoundedIcon";
        }

        public static class Common
        {
            public static string TotalActors => "COMMON.TOTALACTORS";
            public static string TotalDirectors => "COMMON.TOTALDIRECTORS";
            public static string TotalWriters => "COMMON.TOTALWRITERS";
            public static string TotalDiskSpace => "COMMON.TOTALDISKSIZE";
            public static string TotalGenres => "COMMON.TOTALGENRES";
        }

        public static class Shows
        {
            public static string TotalShows => "SHOWS.TOTALSHOWS";
            public static string TotalCompleteCollectedShows => "SHOWS.COMPLETECOLLECTEDSHOWS";
            public static string TotalEpisodes => "SHOWS.TOTALEPISODES";
            public static string TotalMissingEpisodes => "SHOWS.TOTALMISSINGEPISODES";
            public static string TotalPlayLength => "SHOWS.TOTALPLAYLENGTH";
            public static string HighestRatedShow => "SHOWS.HIGHESTRATED";
            public static string LowestRatedShow => "SHOWS.LOWESTRATED";
            public static string OldestPremiered => "SHOWS.OLDESTPREMIERED";
            public static string NewestPremiered => "SHOWS.NEWESTPREMIERED";
            public static string LatestAdded => "SHOWS.LATESTADDED";
            public static string MostEpisodes => "SHOWS.WITHMOESTEPISODES";
            public static string ShowStatusChart => "SHOWS.SHOWSTATUS";
            public static string MostDiskSpace => "SHOWS.MOSTDISKSPACE";
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
            public static string ShowSyncJob => "SHOW-SYNC";
            public static string MovieSyncJob => "MOVIE-SYNC";
            public static string CheckUpdateJob => "UPDATE-CHECKER";
            public static string PingMediaServerJob => "PING";
            public static string SmallMediaServerSyncJob => "SYSTEM-SYNC";
            public static string System => "SYSTEM";
            public static string DatabaseCleanupJob => "DATABASE-CLEANUP";
        }

        public static class JobIds
        {
            public static Guid ShowSyncId => new Guid("be68900b-ee1d-41ef-b12f-60ef3106052e");
            public static Guid MovieSyncId => new Guid("c40555dc-ea57-4c6e-a225-905223d31c3c");
            public static Guid SmallSyncId => new Guid("41e0bf22-1e6b-4f5d-90be-ec966f746a2f");
            public static Guid CheckUpdateId => new Guid("78bc2bf0-abd9-48ef-aeff-9c396d644f2a");
            public static Guid PingEmbyId => new Guid("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e");
            public static Guid DatabaseCleanupId => new Guid("b109ca73-0563-4062-a3e2-f7e6a00b73e9");
        }

        public static class Roles
        {
            public static string Admin => "admin";
            public static string User => "user";
        }

        public static class JwtClaimIdentifiers
        {
            public static string Roles => "roles";
            public static string Id => "id";
        }

        //CHARTS
        public static string CountPerGenre => "COMMON.GENRES";
        public static string CountPerCommunityRating => "COMMON.COMMUNITYRATING";
        public static string CountPerPremiereYear => "COMMON.PREMIEREDATE";
        public static string CountPerCollectedPercentage => "COMMON.COLLECTEDPERCENTAGE";
        public static string CountPerOfficialRating => "COMMON.OFFICIALRATING";

        //COMMON
        public static string Unknown => "UNKNOWN";
        
        //QUALITIES
        public static string FourK => "4K";
        public static string Qhd => "1440P";
        public static string FullHd => "1080P";
        public static string HdReady => "720P";
        public static string FourHunderdEighty => "480P";
        public static string Dvd => "< 480P";

        //DUPLICATE REASONS
        public static string ByImdb => "BYIMDB";
    }
}
