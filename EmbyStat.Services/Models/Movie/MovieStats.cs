using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Movie
{
    public class MovieStats
    {
        public Card MovieCount { get; set; }
        public Card GenreCount { get; set; }
        public Card BoxsetCount { get; set; }
        public Card MostUsedContainer { get; set; }
        public MoviePoster HighestRatedMovie { get; set; }
        public MoviePoster LowestRatedMovie { get; set; }
        public MoviePoster LongestMovie { get; set; }
        public MoviePoster ShortestMovie { get; set; }
        public MoviePoster OldestPremieredMovie { get; set; }
        public MoviePoster YoungestPremieredMovie { get; set; }
        public MoviePoster YoungestAddedMovie { get; set; }
        public Card MostFeaturedMovieActor { get; set; }
        public Card MostFeaturedMovieDirector { get; set; }
        public Card LastPlayedMovie { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
