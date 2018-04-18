using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Movie
{
    public class MovieStatsViewModel
    {
        public CardViewModel MovieCount { get; set; }
        public CardViewModel GenreCount { get; set; }
        public CardViewModel CollectionCount { get; set; }
        public CardViewModel MostUsedContainer { get; set; }
        public CardViewModel HighestRatedMovie { get; set; }
        public CardViewModel LowestRatedMovie { get; set; }
        public CardViewModel LongestMovie { get; set; }
        public CardViewModel ShortestMovie { get; set; }
        public CardViewModel OldestPremieredMovie { get; set; }
        public CardViewModel YoungestPremieredMovie { get; set; }
        public CardViewModel YoungestAddedMovie { get; set; }
        public CardViewModel MostFeaturedMovieActor { get; set; }
        public CardViewModel MostFeaturedMovieDirector { get; set; }
        public CardViewModel LastPlayedMovie { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
