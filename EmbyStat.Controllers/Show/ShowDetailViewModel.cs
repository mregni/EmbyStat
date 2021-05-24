using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Show
{
    public class ShowDetailViewModel
    {
        public string Id { get; set; }
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public int SeasonCount { get; set; }
        public List<VirtualEpisodeViewModel> MissingEpisodes { get; set; }
        public int CollectedEpisodeCount { get; set; }
        public int SpecialEpisodeCount { get; set; }
        public string[] Genres { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public float? CommunityRating { get; set; }
        public long? RunTimeTicks { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string Name { get; set; }
        public int? ProductionYear { get; set; }
        public DateTime? PremiereDate { get; set; }
        public string Path { get; set; }

    }
}
