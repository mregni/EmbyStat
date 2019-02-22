using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Movie
{
    public class MovieGraphsViewModel
    {
        public List<GraphViewModel<SimpleGraphValueViewModel>> BarGraphs { get; set; }
    }
}
