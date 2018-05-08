using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common
{
    public static class Constants
    {
        public static class Movies
        {
            //STATISTICS
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
            public const string MostFeaturedActor = "MOVIES.MOSTFEATUREDACTOR";
            public const string MostFeaturedDirector = "MOVIES.MOSTFEATUREDDIRECTOR";
            public const string MostFeaturedWriter = "MOVIES.MOSTFEATUREDWRITER";
            public const string TotalActors = "MOVIES.TOTALACTORS";
            public const string TotalDirectors = "MOVIES.TOTALDIRECTORS";
            public const string TotalWriters = "MOVIES.TOTALWRITERS";
        }

        //CHARTS
        public const string CountPerGenre = "COMMON.COUNTPERGENRE";
        public const string CountPerCommunityRating = "COMMON.COUNTPERCOMMUNITYRATING";
        public const string CountPerPremiereYear = "COMMON.COUNTPERPREMIEREDATE";

        //COMMON
        public const string Unknown = "UNKNOWN";

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
