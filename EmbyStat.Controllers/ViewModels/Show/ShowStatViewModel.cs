using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Controllers.ViewModels.Show
{
    public class ShowStatViewModel
    {
        public Card ShowCount { get; set; }
        public Card EpisodeCount { get; set; }
        public Card MissingEpisodeCount { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
        public ShowPoster HighestRatedShow { get; set; }
        public ShowPoster LowestRatedShow { get; set; }
        public ShowPoster ShowWithMostEpisodes { get; set; }
        public ShowPoster OldestPremieredShow { get; set; }
        public ShowPoster YoungestPremieredShow { get; set; }
        public ShowPoster YoungestAddedShow { get; set; }
    }
}
