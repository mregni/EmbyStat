using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Show
{
    public class EpisodeViewModel
    {
        public int Id { get; set; }
        public int EpisodeIndex { get; set; }
        public int SeasonIndex { get; set; }
        public string Name { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
