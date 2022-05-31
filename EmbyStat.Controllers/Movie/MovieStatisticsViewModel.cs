using System.Collections.Generic;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Movie;

public class MovieStatisticsViewModel
{
    public List<CardViewModel<string>> Cards { get; set; }
    public List<TopCardViewModel> TopCards { get; set; }
    public List<Chart> Charts { get; set; }
    public List<ShortMovieViewModel> Shorts { get; set; }
    public List<SuspiciousMovieViewModel> NoImdb { get; set; }
    public List<SuspiciousMovieViewModel> NoPrimary { get; set; }
}