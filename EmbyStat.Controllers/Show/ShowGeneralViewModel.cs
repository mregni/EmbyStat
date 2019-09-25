using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Controllers.Show
{
    public class ShowGeneralViewModel
    {
        public CardViewModel<int> ShowCount { get; set; }
        public CardViewModel<int> EpisodeCount { get; set; }
        public CardViewModel<int> MissingEpisodeCount { get; set; }
        public CardViewModel<double> TotalDiskSize { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
        public ShowPoster HighestRatedShow { get; set; }
        public ShowPoster LowestRatedShow { get; set; }
        public ShowPoster ShowWithMostEpisodes { get; set; }
        public ShowPoster OldestPremieredShow { get; set; }
        public ShowPoster NewestPremieredShow { get; set; }
        public ShowPoster LatestAddedShow { get; set; }
    }
}
