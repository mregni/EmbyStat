using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Models.Movie;

namespace EmbyStat.Controllers.Movie
{
    public class MovieStatisticsViewModel
    {
        public MovieGeneralViewModel General { get; set; }
        public MovieChartsViewModel Charts { get; set; }
        public PersonStatsViewModel People { get; set; }
        public SuspiciousTablesViewModel Suspicious { get; set; }
    }
}
