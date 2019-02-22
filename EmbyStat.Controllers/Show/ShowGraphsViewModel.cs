using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Show
{
    public class ShowGraphsViewModel
    {
        public List<GraphViewModel<SimpleGraphValueViewModel>> BarGraphs { get; set; }
        public List<GraphViewModel<SimpleGraphValueViewModel>> PieGraphs { get; set; }
    }
}
