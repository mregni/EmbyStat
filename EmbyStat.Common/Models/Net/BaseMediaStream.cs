using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Net
{
    public class BaseMediaStream
    {
        public int? BitRate { get; set; }
        public int? BitDepth { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string VideoRange { get; set; }
        public string AspectRatio { get; set; }
        public float? AverageFrameRate { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public MediaStreamType Type { get; set; }
    }
}
