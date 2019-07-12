using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Show
{
    public class ShowStatistics
    {
        public ShowGeneral General { get; set; }
        public ShowCharts Charts { get; set; }
        public PersonStats People { get; set; }
    }
}
