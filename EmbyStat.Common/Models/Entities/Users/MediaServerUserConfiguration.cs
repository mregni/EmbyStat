using System;

namespace EmbyStat.Common.Models.Entities.Users;

public class MediaServerUserConfiguration
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public MediaServerUser User { get; set; }
    public bool PlayDefaultAudioTrack { get; set; }
    public string SubtitleLanguagePreference { get; set; }
    public bool DisplayMissingEpisodes { get; set; }
    public string SubtitleMode { get; set; }
    public bool EnableLocalPassword { get; set; }
    public bool HidePlayedInLatest { get; set; }
    public bool RememberAudioSelections { get; set; }
    public bool RememberSubtitleSelections { get; set; }
    public bool EnableNextEpisodeAutoPlay { get; set; }
}