﻿using System;

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
        public static string TotalWatchedMovies => "MOVIES.TOTALWATCHED";
        public static string MostWatchedMovies => "MOVIES.MOSTWATCHED";
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
        public static string MostWatchedShows => "SHOWS.MOSTWATCHED";
        public static string LatestAdded => "SHOWS.LATESTADDED";
        public static string MostEpisodes => "SHOWS.WITHMOESTEPISODES";
        public static string ShowStatusChart => "SHOWS.SHOWSTATUS";
        public static string MostDiskSpace => "SHOWS.MOSTDISKSPACE";
        public static string TotalWatchedEpisodes => "SHOWS.TOTALEPISODESWATCHED";
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