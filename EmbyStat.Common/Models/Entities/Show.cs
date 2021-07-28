using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Show : Extra
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public bool ExternalSynced { get; set; }
        public bool ExternalSyncFailed { get; set; }
        [BsonRef(nameof(Season))]
        public List<Season> Seasons { get; set; }
        [BsonRef(nameof(Episode))]
        public List<Episode> Episodes { get; set; }
    }
}
