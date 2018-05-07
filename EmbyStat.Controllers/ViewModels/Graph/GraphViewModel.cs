using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.ViewModels.Graph
{
    public class GraphViewModel
    {
        public string Title { get; set; }
        public List<BarViewModel> Data { get; set; }
    }
}
