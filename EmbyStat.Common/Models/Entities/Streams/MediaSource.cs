using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;

namespace EmbyStat.Common.Models.Entities.Streams
{
    public class MediaSource
    {
        public string Id { get; set; }
        public int? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public double SizeInMb { get; set; }
        public Movie Movie { get; set; }
        public string MovieId { get; set; }
        public Episode Episode { get; set; }
        public string EpisodeId { get; set; }
    }
}
