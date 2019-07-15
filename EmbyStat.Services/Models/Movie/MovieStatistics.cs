using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Movie
{
    public class MovieStatistics
    {
        public MovieGeneral General { get; set; }
        public MovieCharts Charts { get; set; }
        public PersonStats People { get; set; }
        public SuspiciousTables  Suspicious { get; set; }
    }
}
