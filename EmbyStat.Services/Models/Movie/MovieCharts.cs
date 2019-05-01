
using System.Collections.Generic;
using EmbyStat.Services.Models.Charts;

namespace EmbyStat.Services.Models.Movie
{
    public class MovieCharts
    {
        public List<Chart> BarCharts { get; set; }

        public MovieCharts()
        {
            BarCharts = new List<Chart>();
        }
    }
}
