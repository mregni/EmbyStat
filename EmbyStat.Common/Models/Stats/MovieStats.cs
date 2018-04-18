using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Stats
{
    public class MovieStats
    {
        public Card MovieCount { get; set; }
        public Card GenreCount { get; set; }
        public Card CollectionCount { get; set; }
        public Card MostUsedContainer { get; set; }
        public Card HighestRatedMovie { get; set; }
        public Card LowestRatedMovie { get; set; }
        public Card LongestMovie { get; set; }
        public Card ShortestMovie { get; set; }
        public Card OldestPremieredMovie { get; set; }
        public Card YoungestPremieredMovie { get; set; }
        public Card YoungestAddedMovie { get; set; }
        public Card MostFeaturedMovieActor { get; set; }
        public Card MostFeaturedMovieDirector { get; set; }
        public Card LastPlayedMovie { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
