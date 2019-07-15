using System.Collections.Generic;
using LiteDB;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Session
    {
        [BsonId]
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string ApplicationVersion { get; set; }
        public string Client { get; set; }
        public string DeviceId { get; set; }
        public string AppIconUrl { get; set; }
        public string UserId { get; set; }
        [BsonRef(nameof(Play))]
        public List<Play> Plays { get; set; }
        public Session()
        {
            Plays = new List<Play>();
        }
    }
}
