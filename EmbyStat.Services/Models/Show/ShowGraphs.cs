using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Graph;

namespace EmbyStat.Services.Models.Show
{
    public class ShowGraphs
    {
        public List<Graph<SimpleGraphValue>> BarGraphs { get; set; }
        public List<Graph<SimpleGraphValue>> PieGraphs { get; set; }

        public ShowGraphs()
        {
            BarGraphs = new List<Graph<SimpleGraphValue>>();
            PieGraphs = new List<Graph<SimpleGraphValue>>();
        }
    }
}
