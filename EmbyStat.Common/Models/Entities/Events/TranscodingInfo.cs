using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class TranscodingInfo
    {
        [Key]
        public Guid Id { get; set; }
        public string AudioCodec { get; set; }
        public string VideoCodec { get; set; }
        public string Container { get; set; }
        public bool IsVideoDirect { get; set; }
        public bool IsAudioDirect { get; set; }
        public int? Bitrate { get; set; }
        public float? Framerate { get; set; }
        public double? CompletionPercentage { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? AudioChannels { get; set; }
        public List<TranscodeReason> TranscodeReasons { get; set; }
        public Guid PlayId { get; set; }
        public Play Play { get; set; }
    }
}
