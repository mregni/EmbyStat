using System;

namespace EmbyStat.Controllers.MediaServer;

public class MediaServerUserRowViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset? LastActivityDate { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool IsDisabled { get; set; }
    public int MovieViewCount { get; set; }
    public int EpisodeViewCount { get; set; }
    public int TotalViewCount { get; set; }
}