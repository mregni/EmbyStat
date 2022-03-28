using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;

namespace EmbyStat.Common.Models.Entities.Streams
{
    public class AudioStream
    {
        public string Id { get; set; }
        public int? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public bool IsDefault { get; set; }
        public Movie Movie { get; set; }
        public string MovieId { get; set; }
        public Episode Episode { get; set; }
        public string EpisodeId { get; set; }
    }
}
