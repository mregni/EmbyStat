using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Movie
{
    public class MovieGeneralViewModel
    {
        public CardViewModel<int> MovieCount { get; set; }
        public CardViewModel<int> GenreCount { get; set; }
        public CardViewModel<int> BoxsetCount { get; set; }
        public CardViewModel<string> MostUsedContainer { get; set; }
        public MoviePosterViewModel HighestRatedMovie { get; set; }
        public MoviePosterViewModel LowestRatedMovie { get; set; }
        public MoviePosterViewModel LongestMovie { get; set; }
        public MoviePosterViewModel ShortestMovie { get; set; }
        public MoviePosterViewModel OldestPremieredMovie { get; set; }
        public MoviePosterViewModel NewestPremieredMovie { get; set; }
        public MoviePosterViewModel LatestAddedMovie { get; set; }
        public CardViewModel<string> MostFeaturedMovieActor { get; set; }
        public CardViewModel<string> MostFeaturedMovieDirector { get; set; }
        public CardViewModel<string> LastPlayedMovie { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
