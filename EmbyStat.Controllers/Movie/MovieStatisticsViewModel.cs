using EmbyStat.Controllers.HelperClasses;

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
