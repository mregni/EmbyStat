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
        public Poster HighestRatedMovie { get; set; }
        public Poster LowestRatedMovie { get; set; }
        public Poster LongestMovie { get; set; }
        public Poster ShortestMovie { get; set; }
        public Poster OldestPremieredMovie { get; set; }
        public Poster YoungestPremieredMovie { get; set; }
        public Poster YoungestAddedMovie { get; set; }
        public Card MostFeaturedMovieActor { get; set; }
        public Card MostFeaturedMovieDirector { get; set; }
        public Card LastPlayedMovie { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
