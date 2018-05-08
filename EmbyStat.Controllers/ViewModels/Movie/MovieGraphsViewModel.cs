using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.ViewModels.Graph;
using EmbyStat.Services.Models.Graph;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MovieGraphsViewModel
    {
        public List<GraphViewModel<SimpleGraphValueViewModel>> BarGraphs { get; set; }
    }
}
