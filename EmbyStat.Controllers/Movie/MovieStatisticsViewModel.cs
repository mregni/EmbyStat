using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Controllers.Movie
{
    public class MovieStatisticsViewModel
    {
        public List<CardViewModel<string>> Cards { get; set; }
        public List<TopCardViewModel> TopCards { get; set; }
        public List<Chart> Charts { get; set; }
        public PersonStatsViewModel People { get; set; }
        public List<ShortMovieViewModel> Shorts { get; set; }
        public List<SuspiciousMovieViewModel> NoImdb { get; set; }
        public List<SuspiciousMovieViewModel> NoPrimary { get; set; }
    }
}
