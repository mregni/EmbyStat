using System;

namespace EmbyStat.Common.Models.MediaServer;

public class MediaServerUserRow
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool IsDisabled { get; set; }
    public int MovieViewCount { get; set; }
    public int EpisodeViewCount { get; set; }
    public int TotalViewCount { get; set; }
}