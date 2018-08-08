using System;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models
{
    public class Show : Extra
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public DateTime? DateLastMediaAdded { get; set; }
        public string Status { get; set; }
        public bool TvdbSynced { get; set; }
        public int MissingEpisodesCount { get; set; }
        public bool TvdbFailed { get; set; }
    }
}
