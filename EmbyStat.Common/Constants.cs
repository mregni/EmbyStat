﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common
{
    public static class Constants
    {
        public static class Movies
        {
            public const string TotalMovies = "MOVIES.TOTALMOVIES";
            public const string TotalGenres = "MOVIES.TOTALGENRES";
            public const string LowestRated = "MOVIES.LOWESTRATED";
            public const string HighestRated = "MOVIES.HIGHESTRATED";
            public const string OldestPremiered = "MOVIES.OLDESTPREMIERED";
            public const string YoungestPremiered = "MOVIES.YOUNGESTPREMIERED";
            public const string Shortest = "MOVIES.SHORTEST";
            public const string Longest = "MOVIES.LONGEST";
            public const string YoungestAdded = "MOVIES.YOUNGESTADDED";
            public const string TotalPlayLength = "MOVIES.TOTALPLAYLENGTH";
        }

        public static class Common
        {
            public const string TotalActors = "COMMON.TOTALACTORS";
            public const string TotalDirectors = "COMMON.TOTALDIRECTORS";
            public const string TotalWriters = "COMMON.TOTALWRITERS";
            public const string MostFeaturedActor = "COMMON.MOSTFEATUREDACTOR";
            public const string MostFeaturedDirector = "COMMON.MOSTFEATUREDDIRECTOR";
            public const string MostFeaturedWriter = "COMMON.MOSTFEATUREDWRITER";
        }

        public static class Shows
        {
            public const string TotalShows = "SHOWS.TOTALSHOWS";
            public const string TotalEpisodes = "SHOWS.TOTALEPISODES";
            public const string TotalMissingEpisodes = "SHOWS.TOTALMISSINGEPISODES";
            public const string TotalPlayLength = "SHOWS.TOTALPLAYLENGTH";
            public const string HighestRatedShow = "SHOWS.HIGHESTRATEDSHOW";
            public const string LowestRatedShow = "SHOWS.LOWESTRATEDSHOW";
            public const string OldestPremiered = "SHOWS.OLDESTPREMIERED";
            public const string YoungestPremiered = "SHOWS.YOUNGESTPREMIERED";
            public const string YoungestAdded = "SHOWS.YOUNGESTADDED";
            public const string MostEpisodes = "SHOWS.WITHMOESTEPISODES";
            public const string ShowStatusGraph = "SHOWS.SHOWSTATUSGRAPH";
        }

        public static class Emby
        {
            public const string DeviceName = "EmbyStats server";
            public const string DeviceId = "6d5082f6-dffd-4ce9-8301-03eb339e05d4";
            public const string AppName = "EmbyStats";
            public const string AppVersion = "1.0.0";
            public const string AuthorizationScheme = "MediaBrowser";
        }

        public static class Tvdb
        {
            public const string BaseUrl = "https://api.thetvdb.com";
            public const string SerieEpisodesUrl = "/series/{0}/episodes?page={1}";
            public const string UpdatesUrl = "/updated/query?fromTime={0}&toTime={1}";
            public const string LoginUrl = "/login";
            public const string ApiKey = "BWLRSNRC0AQUIEYX";
        }

        public static class Type
        {
            public const string Movie = "Movie";
        }

        //CHARTS
        public const string CountPerGenre = "COMMON.COUNTPERGENRE";
        public const string CountPerCommunityRating = "COMMON.COUNTPERCOMMUNITYRATING";
        public const string CountPerPremiereYear = "COMMON.COUNTPERPREMIEREDATE";
        public const string CountPerCollectedRate = "COMMON.COUNTPERCOLLECTEDRATE";
        public const string CountPerOfficialRating = "COMMON.COUNTPEROFFICIALRATING";

        //COMMON
        public const string Unknown = "UNKNOWN";

        //SERVER
        public const string TempFolder = "temp";

        //QUALITIES
        public const string FOURK = "4K";
        public const string THOUSANDFOURFOURP = "1440P";
        public const string FULLHD = "1080P";
        public const string HDREADY = "720P";
        public const string FOURHUNDERDEIGHTY = "480P";
        public const string LOWEST = "< 480P";

        //DATABASE
        public const string Actor = "Actor";
        public const string Director = "Director";
        public const string Writer = "Writer";

        //DUPLICATE REASONS
        public const string ByImdb = "BYIMDB";
        public const string ByTitle = "BYTITLE";
    }
}
