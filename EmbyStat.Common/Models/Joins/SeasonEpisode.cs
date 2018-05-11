using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Joins
{
    public class SeasonEpisode
    {
        public string SeasonId { get; set; }
        public Season Season { get; set; }
        public string EpisodeId { get; set; }
        public Episode Episode { get; set; }
    }
}
