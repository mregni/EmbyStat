using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Users;

public class MediaServerUser
{
    public string Id { get; set; }
    public string Name { get; set; }    
    public string ServerId { get; set; }
    public bool HasPassword { get; set; }
    public bool HasConfiguredPassword { get; set; }
    public bool HasConfiguredEasyPassword { get; set; }
    public string PrimaryImageTag { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public DateTimeOffset? LastActivityDate { get; set; }
    
    public bool PlayDefaultAudioTrack { get; set; }
    public string SubtitleLanguagePreference { get; set; }
    public bool DisplayMissingEpisodes { get; set; }
    public string SubtitleMode { get; set; }
    
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool IsHiddenRemotely { get; set; }
    public bool IsHiddenFromUnusedDevices { get; set; }
    public bool IsDisabled { get; set; }
    public bool EnableLiveTvAccess { get; set; }
    public bool EnableContentDeletion { get; set; }
    public bool EnableContentDownloading { get; set; }
    public bool EnableSubtitleDownloading { get; set; }
    public bool EnableSubtitleManagement { get; set; }
    public bool EnableSyncTranscoding { get; set; }
    public bool EnableMediaConversion { get; set; }
    public int InvalidLoginAttemptCount { get; set; }
    public bool EnablePublicSharing { get; set; }
    public int RemoteClientBitrateLimit { get; set; }
    public int SimultaneousStreamLimit { get; set; }
    public bool EnableAllDevices { get; set; }
    public ICollection<MediaServerUserView> Views { get; set; }
}
