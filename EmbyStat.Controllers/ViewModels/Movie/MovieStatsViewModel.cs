using EmbyStat.Controllers.ViewModels.Stat;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MovieStatsViewModel
    {
        public CardViewModel MovieCount { get; set; }
        public CardViewModel GenreCount { get; set; }
        public CardViewModel BoxsetCount { get; set; }
        public CardViewModel MostUsedContainer { get; set; }
        public PosterViewModel HighestRatedMovie { get; set; }
        public PosterViewModel LowestRatedMovie { get; set; }
        public PosterViewModel LongestMovie { get; set; }
        public PosterViewModel ShortestMovie { get; set; }
        public PosterViewModel OldestPremieredMovie { get; set; }
        public PosterViewModel YoungestPremieredMovie { get; set; }
        public PosterViewModel YoungestAddedMovie { get; set; }
        public CardViewModel MostFeaturedMovieActor { get; set; }
        public CardViewModel MostFeaturedMovieDirector { get; set; }
        public CardViewModel LastPlayedMovie { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
