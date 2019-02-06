using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Play
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string MediaId { get; set; }
        public ICollection<PlayState> PlayStates { get; set; }
        public ICollection<TranscodingInfo> TranscodingInfos { get; set; }
        public bool IsFinished { get; set; }
        public string SessionId { get; set; }
        public Session Session { get; set; }
    }
}
