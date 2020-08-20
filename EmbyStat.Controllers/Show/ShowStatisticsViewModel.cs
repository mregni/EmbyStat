using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Show
{
    public class ShowStatisticsViewModel
    {
        public List<CardViewModel<string>> Cards { get; set; }
        public List<TopCardViewModel> TopCards { get; set; }
        public List<ChartViewModel> BarCharts { get; set; }
        public List<ChartViewModel> PieCharts { get; set; }
        public PersonStatsViewModel People { get; set; }
    }
}