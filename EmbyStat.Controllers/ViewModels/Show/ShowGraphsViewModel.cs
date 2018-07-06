using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.ViewModels.Graph;

namespace EmbyStat.Controllers.ViewModels.Show
{
    public class ShowGraphsViewModel
    {
        public List<GraphViewModel<SimpleGraphValueViewModel>> BarGraphs { get; set; }
        public List<GraphViewModel<SimpleGraphValueViewModel>> PieGraphs { get; set; }
    }
}
