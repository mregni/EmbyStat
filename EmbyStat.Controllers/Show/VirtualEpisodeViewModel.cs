using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Show
{
    public class VirtualSeasonViewModel
    {
        public int SeasonNumber { get; set; }
        public IEnumerable<VirtualEpisodeViewModel> Episodes { get; set; }
    }

    public class VirtualEpisodeViewModel
    {
        public int Id { get; set; }
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
