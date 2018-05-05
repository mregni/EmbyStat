using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common
{
    public static class Constants
    {
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

        //MOVIE STATS
        public const string MoviesTotalMovies = "MOVIES.TOTALMOVIES";
        public const string MoviesTotalGenres = "MOVIES.TOTALGENRES";
        public const string MoviesLowestRated = "MOVIES.LOWESTRATED";
        public const string MoviesHighestRated = "MOVIES.HIGHESTRATED";
        public const string MoviesOldestPremiered = "MOVIES.OLDESTPREMIERED";
        public const string MoviesYoungestPremiered = "MOVIES.YOUNGESTPREMIERED";
        public const string MoviesShortest = "MOVIES.SHORTEST";
        public const string MoviesLongest = "MOVIES.LONGEST";
        public const string MoviesYoungestAdded = "MOVIES.YOUNGESTADDED";
        public const string MoviesTotalPlayLength = "MOVIES.TOTALPLAYLENGTH";
        public const string MoviesMostFeaturedActor = "MOVIES.MOSTFEATUREDACTOR";
        public const string MoviesMostFeaturedDirector = "MOVIES.MOSTFEATUREDDIRECTOR";
        public const string MoviesMostFeaturedWriter = "MOVIES.MOSTFEATUREDWRITER";
        public const string MoviesTotalActors = "MOVIES.TOTALACTORS";
        public const string MoviesTotalDirectors = "MOVIES.TOTALDIRECTORS";
        public const string MoviesTotalWriters = "MOVIES.TOTALWRITERS";

        //DUPLICATE REASONS
        public const string ByImdb = "BYIMDB";
        public const string ByTitle = "BYTITLE";
    }
}
