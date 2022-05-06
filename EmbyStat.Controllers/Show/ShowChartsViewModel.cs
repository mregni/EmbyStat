using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Show;

public class ShowChartsViewModel
{
    public List<ChartViewModel> BarCharts { get; set; }
    public List<ChartViewModel> PieCharts { get; set; }
}