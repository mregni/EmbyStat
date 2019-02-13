using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Controllers.Show
{
    public class ShowStatViewModel
    {
        public Card<int> ShowCount { get; set; }
        public Card<int> EpisodeCount { get; set; }
        public Card<int> MissingEpisodeCount { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
        public ShowPoster HighestRatedShow { get; set; }
        public ShowPoster LowestRatedShow { get; set; }
        public ShowPoster ShowWithMostEpisodes { get; set; }
        public ShowPoster OldestPremieredShow { get; set; }
        public ShowPoster YoungestPremieredShow { get; set; }
        public ShowPoster YoungestAddedShow { get; set; }
    }
}
