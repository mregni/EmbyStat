using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class AudioStream
    {
        [Key]
        public string Id { get; set; }
        public long? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public Video Video { get; set; }
        public string VideoId { get; set; }
    }
}
