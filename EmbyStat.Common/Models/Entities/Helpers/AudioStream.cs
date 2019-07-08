using LiteDB;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class AudioStream
    {
        [BsonId]
        public string Id { get; set; }
        public long? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public string VideoId { get; set; }
    }
}
