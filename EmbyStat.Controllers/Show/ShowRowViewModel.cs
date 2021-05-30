using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Show
{
    public class ShowRowViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SortName { get; set; }
        public int MissingEpisodesCount { get; set; }
        public int CollectedEpisodeCount { get; set; }
        public int SpecialEpisodeCount { get; set; }
        public string Status { get; set; }
        public long? CumulativeRunTimeTicks { get; set; }
        public string[] Genres { get; set; }
        public string OfficialRating { get; set; }
        public long? RunTime { get; set; }
    }
}
