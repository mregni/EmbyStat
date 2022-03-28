using System;

namespace EmbyStat.Common.Models.Entities
{
    public class MediaServerStatus
    {
        public Guid Id { get; set; }
        public int MissedPings { get; set; }
    }
}
