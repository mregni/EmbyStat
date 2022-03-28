using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Shows
{
    public class Episode : Video
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public LocationType LocationType { get; set; }
        public Season Season { get; set; }
        public string SeasonId { get; set; }
    }
}
