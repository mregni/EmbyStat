using System.Collections.Generic;

namespace EmbyStat.Common.Models.Show
{
    public class VirtualSeason
    {
        public int SeasonNumber { get; set; }
        public IEnumerable<VirtualEpisode> Episodes { get; set; }
    }
}
