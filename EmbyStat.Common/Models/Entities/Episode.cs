using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class Episode: Video
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
	    public int? IndexNumber { get; set; }
	    public int? IndexNumberEnd { get; set; }
        public string ShowName { get; set; }
        public string ShowId { get; set; }
        public LocationType LocationType { get; set; }
    }
}
