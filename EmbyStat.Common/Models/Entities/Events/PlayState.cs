using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class PlayState
    {
        [Key]
        public Guid Id { get; set; }
        public long? PositionTicks { get; set; }
        public int? VolumeLevel { get; set; }
        public string MediaSourceId { get; set; }
        public PlayMethod? PlayMethod { get; set; }
        public RepeatMode RepeatMode { get; set; }
        public bool IsTranscoding { get; set; }
        public bool IsPaused { get; set; }
        public List<TranscodeReason> TranscodeReasons { get; set; }
        public string AudioCodec { get; set; }
        public string VideoCodec { get; set; }
        public DateTime TimeLogged { get; set; }
        public Guid PlayId { get; set; }
        public Play Play { get; set; }
    }
}