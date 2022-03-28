using System;

namespace EmbyStat.Common.Models.Entities.Users;

public class MediaServerUserPolicy
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public MediaServerUser User { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool IsHiddenRemotely { get; set; }
    public bool IsHiddenFromUnusedDevices { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsTagBlockingModeInclusive { get; set; }
    public bool EnableUserPreferenceAccess { get; set; }
    public bool EnableRemoteControlOfOtherUsers { get; set; }
    public bool EnableSharedDeviceControl { get; set; }
    public bool EnableRemoteAccess { get; set; }
    public bool EnableLiveTvManagement { get; set; }
    public bool EnableLiveTvAccess { get; set; }
    public bool EnableMediaPlayback { get; set; }
    public bool EnableAudioPlaybackTranscoding { get; set; }
    public bool EnableVideoPlaybackTranscoding { get; set; }
    public bool EnablePlaybackRemuxing { get; set; }
    public bool EnableContentDeletion { get; set; }
    public bool EnableContentDownloading { get; set; }
    public bool EnableSubtitleDownloading { get; set; }
    public bool EnableSubtitleManagement { get; set; }
    public bool EnableSyncTranscoding { get; set; }
    public bool EnableMediaConversion { get; set; }
    public bool EnableAllChannels { get; set; }
    public bool EnableAllFolders { get; set; }
    public int InvalidLoginAttemptCount { get; set; }
    public bool EnablePublicSharing { get; set; }
    public int RemoteClientBitrateLimit { get; set; }
    public string AuthenticationProviderId { get; set; }
    public int SimultaneousStreamLimit { get; set; }
    public bool EnableAllDevices { get; set; }
}