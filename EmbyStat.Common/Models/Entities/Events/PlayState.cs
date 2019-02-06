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
        public bool CanSeek { get; set; }
        public bool IsPaused { get; set; }
        public bool IsMuted { get; set; }
        public int? VolumeLevel { get; set; }
        public int? AudioStreamIndex { get; set; }
        public int? SubtitleStreamIndex { get; set; }
        public string MediaSourceId { get; set; }
        public PlayMethod? PlayMethod { get; set; }
        public RepeatMode RepeatMode { get; set; }
        public Guid PlayId { get; set; }
        public Play Play { get; set; }
    }
}
