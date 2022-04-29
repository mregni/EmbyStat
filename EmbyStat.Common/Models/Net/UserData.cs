using System;

namespace EmbyStat.Common.Models.Net;

public class UserData
{
    public long PlaybackPositionTicks { get; set; }
    public int PlayCount { get; set; }

    public bool IsFavorite { get; set; }
    public DateTime LastPlayedDate { get; set; }

    public bool Played { get; set; }
}