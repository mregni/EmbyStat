using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class Show : Extra
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public bool TvdbSynced { get; set; }
        public int MissingEpisodesCount { get; set; }
        public bool TvdbFailed { get; set; }
    }
}
