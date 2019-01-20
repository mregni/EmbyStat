using System.Collections.Generic;
using EmbyStat.Controllers.ViewModels.Graph;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MovieGraphsViewModel
    {
        public List<GraphViewModel<SimpleGraphValueViewModel>> BarGraphs { get; set; }
    }
}
