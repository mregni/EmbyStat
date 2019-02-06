using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Session
    {
        [Key]
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string ApplicationVersion { get; set; }
        public string Client { get; set; }
        public List<string> PlayableMediaTypes { get; set; }
        public string DeviceId { get; set; }
        public string AppIconUrl { get; set; }
        public ICollection<Play> Plays { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
