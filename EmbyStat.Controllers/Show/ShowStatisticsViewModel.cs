using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Show
{
    public class ShowStatisticsViewModel
    {
        public ShowGeneralViewModel General { get; set; }
        public ShowChartsViewModel Charts { get; set; }
        public PersonStatsViewModel People { get; set; }
    }
}