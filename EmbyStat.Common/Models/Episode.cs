using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models
{
    public class Episode: Video
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
	    public int? IndexNumber { get; set; }
	    public int? IndexNumberEnd { get; set; }
	}
}
