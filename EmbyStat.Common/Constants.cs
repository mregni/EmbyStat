using System;

namespace EmbyStat.Common;

public static class Constants
{
    public static class Tables
    {
        public static string Movies => "Movies";
        public static string MediaSources => "MediaSource";
        public static string VideoStreams => "VideoStream";
        public static string AudioStreams => "AudioStream";
        public static string SubtitleStreams => "SubtitleStream";
        public static string GenreMovie => "GenreMovie";
        public static string MediaPerson => "MediaPerson";
        public static string People => "People";
        public static string Genres => "Genres";
        public static string Shows => "Shows";
        public static string Seasons => "Season";
        public static string Episodes => "Episodes";
        public static string GenreShow => "GenreShow";
        public static string MediaServerUsers => "MediaServerUsers";
        public static string MediaServerUserViews => "MediaServerUserViews";
        public static string Sessions => "Sessions";
        public static string MediaPlays => "MediaPlays";
    }

    public static class MediaServer
    {
        public static string TotalActiveUsers => "USERS.ACTIVEUSERS";
        public static string TotalIdleUsers => "USERS.IDLEUSERS";
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
        public static string PlayedPlayLength => "MOVIES.PLAYEDPLAYLENGTH";
        public static string TotalWatchedMovies => "MOVIES.TOTALWATCHED";
        public static string MostWatchedMovies => "MOVIES.MOSTWATCHED";
        public static string WatchedPerHour => "MOVIES.WATCHEDPERHOUR";
        public static string DaysOfTheWeek => "SHOWS.DAYSOFTHEWEEK";
        public static string CurrentPlayingCount => "SHOWS.CURRENTPLAYINGCOUNT";
    }

    public static class Icons
    {
        public static string TheatersRoundedIcon => "TheatersRoundedIcon";
        public static string StorageRoundedIcon => "StorageRoundedIcon";
        public static string QueryBuilderRoundedIcon => "QueryBuilderRoundedIcon";
        public static string PoundRoundedIcon => "PoundRoundedIcon";
        public static string PeopleAltRoundedIcon => "PeopleAltRoundedIcon";
        public static string PlayRoundedIcon => "PlayCircleOutline";
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
        public static string MostWatchedShows => "SHOWS.MOSTWATCHED";
        public static string LatestAdded => "SHOWS.LATESTADDED";
        public static string MostEpisodes => "SHOWS.WITHMOESTEPISODES";
        public static string ShowStatusChart => "SHOWS.SHOWSTATUS";
        public static string MostDiskSpace => "SHOWS.MOSTDISKSPACE";
        public static string TotalWatchedEpisodes => "SHOWS.TOTALEPISODESWATCHED";
        public static string PlayedPlayLength => "SHOWS.PLAYEDPLAYLENGTH";
        public static string CurrentPlayingCount => "SHOWS.CURRENTPLAYINGCOUNT";
        public static string WatchedPerHour => "SHOWS.WATCHEDPERHOUR";
        public static string DaysOfTheWeek => "SHOWS.DAYSOFTHEWEEK";
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
        public static Guid ShowSyncId => new("be68900b-ee1d-41ef-b12f-60ef3106052e");
        public static Guid MovieSyncId => new("c40555dc-ea57-4c6e-a225-905223d31c3c");
        public static Guid SmallSyncId => new("41e0bf22-1e6b-4f5d-90be-ec966f746a2f");
        public static Guid CheckUpdateId => new("78bc2bf0-abd9-48ef-aeff-9c396d644f2a");
        public static Guid PingEmbyId => new("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e");
        public static Guid DatabaseCleanupId => new("b109ca73-0563-4062-a3e2-f7e6a00b73e9");
    }

    public static class StatisticPageIds
    {
        public static Guid MoviePage => new("85f1dad1-89ff-4f61-8225-6b0e6c7fe2cf");
        public static Guid ShowPage => new("14928b72-f248-4442-b1ce-2c0f96eb543b");
    }

    public static class MovieStatisticsIds
    {
        public static Guid MovieTotalCount => new("bded12ef-e42c-403f-991b-5ec13b73999c");
        public static Guid MovieTotalGenreCount => new("09dec73b-09e5-4fe4-a832-a406c6a2e092");
        public static Guid MovieTotalPlayLength => new("8ba55ff0-5207-49ba-b2a6-e8d2245b3bdb");
        public static Guid MovieTotalDiskSpaceUsage => new("8bc37492-b276-4753-8a9a-7ce79b87751d");
        public static Guid MovieTotalWatchedCount => new("14d27cf8-99b6-4a7f-949d-31517326fd8e");
        public static Guid MovieTotalWatchedTime => new("79aa2d55-fd78-41ff-b57f-1eee65f66630");
        public static Guid MovieTotalActorCount => new("34666222-cb3f-47f9-a19b-afe3a804bee6");
        public static Guid MovieTotalDirectorCount => new("7f98a5e3-6f58-4715-ba7e-1584fd885f54");
        public static Guid MovieTotalWriterCount => new("7e34121e-5df0-43d9-880a-32b4e4a871c7");
        public static Guid MovieHighestRatedList => new("ff59deb6-c924-4d1c-8e8a-2d7285eed16d");
        public static Guid MovieLowestRatedList => new("54b66037-2f79-4ad9-9185-25a2d22d32bc");
        public static Guid MovieOldestPremieredList => new("4c499011-a730-4425-9a29-2b54a459a6f6");
        public static Guid MovieNewestPremieredList => new("37228821-9e10-4c18-8fa6-54671e3d6321");
        public static Guid MovieShortestList => new("d1ce35dd-6fcd-4ded-b673-b072a6cda3b1");
        public static Guid MovieLongestList => new("55094d0a-8628-4dd7-9b76-ecef09039f51");
        public static Guid MovieLatestAddedList => new("23a066fd-986a-40e1-acc6-e7f074bb6e8f");
        public static Guid MovieMostWatchedList => new("7724c480-05d6-4ac1-a3d2-a2c828774484");
        public static Guid MovieGenreChart => new("593e9198-7a63-45a0-b77b-718de127ef58");
        public static Guid MovieRatingChart => new("668c3a84-5e04-42e1-8e72-4b2d341f88ef");
        public static Guid MoviePremiereYearChart => new("b0a5db94-e6c7-45d4-abec-c2caa8576f22");
        public static Guid MovieOfficialRatingChart => new("90542574-f6df-4c08-8ed0-70f63c01f035");
        public static Guid MovieWatchedPerHourOfDayChart => new("9c65c3cf-4d2b-4b4f-947a-4b33482508f6");
        public static Guid MovieWatchedPerDayOfWeekChart => new("8ed8db46-5534-4845-85b5-1b0c7458c57e");
        public static Guid MovieTotalCurrentWatchingCount => new("4845db46-5534-8ed8-85b5-1b0c7458c57e");
    }

    public static class ShowStatisticsIds
    {
        public static Guid ShowTotalCount => new("90adfb53-285a-4073-a05d-95a2b70bb42a");
        public static Guid ShowTotalGenreCount => new("3f3bd838-7c6e-405f-99e5-4219218fce21");
        public static Guid ShowCompleteCollectedCount => new("2abe896e-e405-40d4-a714-f47784cdc8d5");
        public static Guid ShowTotalEpisodesCount => new("24e710b9-0804-43aa-8eab-8e789d7de3c7");
        public static Guid ShowTotalMissingEpisodeCount => new("39ec1232-d338-445d-bdfb-fe409685ea83");
        public static Guid ShowTotalPlayLength => new("0fe36820-4b06-4aa2-975c-61dba003d056");
        public static Guid ShowTotalDiskSpaceUsage => new("0664159c-25e7-4ed0-b1ee-73b2b5e4984d");
        public static Guid ShowTotalWatchedEpisodeCount => new("87d8ff0b-55a1-46b2-b6f9-b632d7121961");
        public static Guid ShowTotalWatchedTime => new("1f33ccfe-1658-4679-9cf2-b4b689d3f381");
        public static Guid ShowTotalActorCount => new("58cbdff6-2444-41ca-9fbb-a82e18f447a9");
        public static Guid ShowNewestPremieredList => new("663b55b4-f83e-40ec-bc15-a025d72b730d");
        public static Guid ShowOldestPremieredList => new("03620638-bb90-416b-a232-f2e0608befc6");
        public static Guid ShowLatestAddedList => new("b6068b78-bdf2-47be-a154-b88b4ce04fc3");
        public static Guid ShowHighestRatedList => new("4399c72b-dccd-4fa8-934a-c62a51001f59");
        public static Guid ShowLowestRatedList => new("6c4f64ab-2a4f-41ce-94fe-d9a09fc6ca10");
        public static Guid ShowWithMostEpisodesList => new("ae4107f9-9bfd-4f50-8301-56726e6c08d9");
        public static Guid ShowMostDiskUsageList => new("1d1c1782-282c-49bd-b34f-dc4e1673ce0e");
        public static Guid ShowMostWatchedList => new("5e70e8a0-699c-4a2f-81ce-b1feb79d09e0");
        public static Guid ShowGenreChart => new("7d9a181c-7df8-4b0c-9f55-65ea77465398");
        public static Guid ShowRatingChart => new("3d4f964b-3ce7-4a5c-ae44-fbc5f21f5be9");
        public static Guid ShowPremiereYearChart => new("8f55410d-681e-4b7d-9118-5cca62bf6b2c");
        public static Guid ShowCollectedRateChart => new("859203d1-2ab6-4a3e-9665-4e55dae68e08");
        public static Guid ShowOfficialRatingChart => new("a8180a05-3994-4792-ab36-e71f8fa72c8d");
        public static Guid EpisodeWatchedPerHourOfDayChart => new("f4cfda68-77ad-417a-b042-da166ab20e42");
        public static Guid EpisodeWatchedPerDayOfWeekChart => new("8a851cd0-d1ea-4b8b-a3ca-91f411bbcf91");
        public static Guid ShowStateChart => new("d1ea1cd0-8a85-4b8b-a3ca-91f411bbcf91");
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

    public static class Users
    {
        public static string TotalWatchedEpisodes => "USERS.WATCHED.EPISODES";
        public static string TotalWatchedMovies => "USERS.WATCHED.MOVIES";
    }
}