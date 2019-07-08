using System;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class EmbyStatus
    {
        [BsonId]
        public Guid Id { get; set; }
        public int MissedPings { get; set; }
    }
}
