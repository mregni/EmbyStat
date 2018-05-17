using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class VirtualEpisode
    {
        public int SeasonIndex { get; set; }
        public int EpisodeIndex { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
