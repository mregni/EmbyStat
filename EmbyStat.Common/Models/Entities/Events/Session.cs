using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Session
    {
        [Key]
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string ApplicationVersion { get; set; }
        public string Client { get; set; }
        public string DeviceId { get; set; }
        public string AppIconUrl { get; set; }
        public ICollection<Play> Plays { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public Session()
        {
            Plays = new List<Play>();
        }
    }
}
