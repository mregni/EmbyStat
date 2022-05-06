using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Net;

public class BaseUserDto
{
    public string Name { get; set; }
    public string ServerId { get; set; }
    public string Id { get; set; }
    public bool HasPassword { get; set; }
    public bool HasConfiguredPassword { get; set; }
    public bool HasConfiguredEasyPassword { get; set; }
    public DateTimeOffset LastLoginDate { get; set; }
    public DateTimeOffset LastActivityDate { get; set; }
    public BaseUserConfiguration Configuration { get; set; }
    public BaseUserPolicy Policy { get; set; }
    public string ConnectUserName { get; set; }
    public string ConnectLinkType { get; set; }
    public string PrimaryImageTag { get; set; }
    public double? PrimaryImageAspectRatio { get; set; }
}

public class BaseUserConfiguration
    {
        public bool PlayDefaultAudioTrack { get; set; }
        public string SubtitleLanguagePreference { get; set; }
        public bool DisplayMissingEpisodes { get; set; }
        public string SubtitleMode { get; set; }
        public bool EnableLocalPassword { get; set; }
        public List<string> OrderedViews { get; set; }
        public List<object> LatestItemsExcludes { get; set; }
        public List<string> MyMediaExcludes { get; set; }
        public bool HidePlayedInLatest { get; set; }
        public bool RememberAudioSelections { get; set; }
        public bool RememberSubtitleSelections { get; set; }
        public bool EnableNextEpisodeAutoPlay { get; set; }
        public string AudioLanguagePreference { get; set; }
    }

    public class BaseUserPolicy
    {
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsHiddenRemotely { get; set; }
        public bool IsHiddenFromUnusedDevices { get; set; }
        public bool IsDisabled { get; set; }
        public List<object> BlockedTags { get; set; }
        public bool IsTagBlockingModeInclusive { get; set; }
        public bool EnableUserPreferenceAccess { get; set; }
        public List<object> AccessSchedules { get; set; }
        public List<object> BlockUnratedItems { get; set; }
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
        public List<object> EnableContentDeletionFromFolders { get; set; }
        public bool EnableContentDownloading { get; set; }
        public bool EnableSubtitleDownloading { get; set; }
        public bool EnableSubtitleManagement { get; set; }
        public bool EnableSyncTranscoding { get; set; }
        public bool EnableMediaConversion { get; set; }
        public List<object> EnabledChannels { get; set; }
        public bool EnableAllChannels { get; set; }
        public List<object> EnabledFolders { get; set; }
        public bool EnableAllFolders { get; set; }
        public int InvalidLoginAttemptCount { get; set; }
        public bool EnablePublicSharing { get; set; }
        public int RemoteClientBitrateLimit { get; set; }
        public string AuthenticationProviderId { get; set; }
        public List<object> ExcludedSubFolders { get; set; }
        public int SimultaneousStreamLimit { get; set; }
        public List<object> EnabledDevices { get; set; }
        public bool EnableAllDevices { get; set; }
    }