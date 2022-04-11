using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;

namespace EmbyStat.Common.Models.Entities.Streams;

public class SubtitleStream
{
    public string Id { get; set; }
    public string Codec { get; set; }
    public string DisplayTitle { get; set; }
    public bool IsDefault { get; set; }
    public string Language { get; set; }
    public Movie Movie { get; set; }
    public string MovieId { get; set; }
    public Episode Episode { get; set; }
    public string EpisodeId { get; set; }
}