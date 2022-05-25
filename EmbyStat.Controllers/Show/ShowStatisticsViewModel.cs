using System.Collections.Generic;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Show;

public class ShowStatisticsViewModel
{
    public List<CardViewModel<string>> Cards { get; set; }
    public List<TopCardViewModel> TopCards { get; set; }
    public List<Chart> BarCharts { get; set; }
    public List<Chart> PieCharts { get; set; }
}