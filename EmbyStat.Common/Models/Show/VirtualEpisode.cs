using System;

namespace EmbyStat.Common.Models.Show;

public class VirtualEpisode
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public DateTime? FirstAired { get; set; }
}