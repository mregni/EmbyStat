using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Movie
{
    public class MovieGeneral
    {
        public Card<int> MovieCount { get; set; }
        public Card<int> GenreCount { get; set; }
        public Card<int> BoxsetCount { get; set; }
        public Card<string> MostUsedContainer { get; set; }
        public MoviePoster HighestRatedMovie { get; set; }
        public MoviePoster LowestRatedMovie { get; set; }
        public MoviePoster LongestMovie { get; set; }
        public MoviePoster ShortestMovie { get; set; }
        public MoviePoster OldestPremieredMovie { get; set; }
        public MoviePoster NewestPremieredMovie { get; set; }
        public MoviePoster LatestAddedMovie { get; set; }
        public Card<string> MostFeaturedMovieActor { get; set; }
        public Card<string> MostFeaturedMovieDirector { get; set; }
        public Card<string> LastPlayedMovie { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
