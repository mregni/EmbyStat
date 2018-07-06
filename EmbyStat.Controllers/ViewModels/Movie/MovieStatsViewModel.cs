using EmbyStat.Controllers.ViewModels.Stat;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MovieStatsViewModel
    {
        public CardViewModel MovieCount { get; set; }
        public CardViewModel GenreCount { get; set; }
        public CardViewModel BoxsetCount { get; set; }
        public CardViewModel MostUsedContainer { get; set; }
        public MoviePosterViewModel HighestRatedMovie { get; set; }
        public MoviePosterViewModel LowestRatedMovie { get; set; }
        public MoviePosterViewModel LongestMovie { get; set; }
        public MoviePosterViewModel ShortestMovie { get; set; }
        public MoviePosterViewModel OldestPremieredMovie { get; set; }
        public MoviePosterViewModel YoungestPremieredMovie { get; set; }
        public MoviePosterViewModel YoungestAddedMovie { get; set; }
        public CardViewModel MostFeaturedMovieActor { get; set; }
        public CardViewModel MostFeaturedMovieDirector { get; set; }
        public CardViewModel LastPlayedMovie { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
