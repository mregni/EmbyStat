using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.SqLite.Streams
{
    public class SqlSubtitleStream
    {
        public int Id { get; set; }
        public string Codec { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string Language { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlEpisode Episode { get; set; }
        public string EpisodeId { get; set; }
    }
}
