using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Graph
{
    public class Graph
    {
        public string Title { get; set; }
        public List<Bar> Data { get; set; }
    }
}
