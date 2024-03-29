﻿using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;

namespace EmbyStat.Common.Models.Entities.Streams;

public class VideoStream
{
    public string Id { get; set; }
    public string AspectRatio { get; set; }
    public float? AverageFrameRate { get; set; }
    public int? BitRate { get; set; }
    public int? Channels { get; set; }
    public int? Height { get; set; }
    public string Language { get; set; }
    public int? Width { get; set; }
    public int? BitDepth { get; set; }
    public string Codec { get; set; }
    public bool IsDefault { get; set; }
    public string VideoRange { get; set; }
    public Movie Movie { get; set; }
    public string MovieId { get; set; }
    public Episode Episode { get; set; }
    public string EpisodeId { get; set; }
}