using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Show : Extra
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public bool TvdbSynced { get; set; }
        public int MissingEpisodesCount { get; set; }
        public bool TvdbFailed { get; set; }
        [BsonRef(nameof(Season))]
        public IEnumerable<Season> Seasons { get; set; }
        [BsonRef(nameof(Episode))]
        public IEnumerable<Episode> Episodes { get; set; }
    }
}
