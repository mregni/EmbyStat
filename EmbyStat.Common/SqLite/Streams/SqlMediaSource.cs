using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.SqLite.Streams
{
    public class SqlMediaSource
    {
        public string Id { get; set; }
        public int? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public double SizeInMb { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlEpisode Episode { get; set; }
        public string EpisodeId { get; set; }
    }
}
