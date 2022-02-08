using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlEpisode : SqlVideo
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public LocationType LocationType { get; set; }
        public SqlSeason Season { get; set; }
        public string SeasonId { get; set; }
    }
}
