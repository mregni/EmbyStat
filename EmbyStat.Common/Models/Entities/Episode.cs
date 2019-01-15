using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Joins;

namespace EmbyStat.Common.Models.Entities
{
    public class Episode: Video
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
	    public int? IndexNumber { get; set; }
	    public int? IndexNumberEnd { get; set; }
        public ICollection<SeasonEpisode> SeasonEpisodes { get; set; }
    }
}
