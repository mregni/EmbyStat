using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.SqLite.Streams
{
    public class SqlAudioStream
    {
        public int Id { get; set; }
        public int? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public bool IsDefault { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlEpisode Episode { get; set; }
        public string EpisodeId { get; set; }
    }
}
