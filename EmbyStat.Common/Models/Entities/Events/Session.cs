using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Session
    {
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string ApplicationVersion { get; set; }
        public string Client { get; set; }
        public string DeviceId { get; set; }
        public string AppIconUrl { get; set; }
        public string UserId { get; set; }
        public List<Play> Plays { get; set; }
        public Session()
        {
            Plays = new List<Play>();
        }
    }
}
