namespace EmbyStat.Controllers.HelperClasses.Streams
{
    public class AudioStreamViewModel
    {
        public string Id { get; set; }
        public int? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public bool IsDefault { get; set; }
    }
}
