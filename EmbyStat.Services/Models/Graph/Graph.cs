using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Graph
{
    public class Graph<T> where T : class
    {
        public string Title { get; set; }
        public List<T> Data { get; set; }
    }
}
