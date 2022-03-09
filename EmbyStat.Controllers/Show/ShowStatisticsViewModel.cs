using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Models.Charts;

namespace EmbyStat.Controllers.Show
{
    public class ShowStatisticsViewModel
    {
        public List<CardViewModel<string>> Cards { get; set; }
        public List<TopCardViewModel> TopCards { get; set; }
        public List<Chart> BarCharts { get; set; }
        public List<Chart> PieCharts { get; set; }
    }
}