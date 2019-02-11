using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmbyStat.Common.Models.Entities
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ServerId { get; set; }
        public bool HasPassword { get; set; }
        public bool HasConfiguredPassword { get; set; }
        public bool HasConfiguredEasyPassword { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? LastActivityDate { get; set; }
        public bool PlayDefaultAudioTrack { get; set; }
        public bool DisplayMissingEpisodes { get; set; }
        public string SubtitleMode { get; set; }
        public bool DisplayCollectionsView { get; set; }
        public bool EnableLocalPassword { get; set; }
        public bool HidePlayedInLatest { get; set; }
        public bool RememberAudioSelections { get; set; }
        public bool RememberSubtitleSelections { get; set; }
        public bool EnableNextEpisodeAutoPlay { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public long? MaxParentalRating { get; set; }
        public List<string> BlockedTags { get; set; }
        public bool EnableUserPreferenceAccess { get; set; }
        public ICollection<UserAccessSchedule> AccessSchedules { get; set; }
        public List<string> BlockUnratedItems { get; set; }
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
        public List<string> EnabledDevices { get; set; }
        public bool EnableAllDevices { get; set; }
        public List<string> EnabledChannels { get; set; }
        public bool EnableAllChannels { get; set; }
        public List<string> EnabledFolders { get; set; }
        public bool EnableAllFolders { get; set; }
        public long InvalidLoginAttemptCount { get; set; }
        public bool EnablePublicSharing { get; set; }
        public long RemoteClientBitRateLimit { get; set; }
        public string AuthenticationProviderId { get; set; }
        public List<string> ExcludedSubFolders { get; set; }
        public bool DisablePremiumFeatures { get; set; }
        public bool Deleted { get; set; }
        public string PrimaryImageTag { get; set; }
    }

    public class UserAccessSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; }
        public long StartHour { get; set; }
        public long EndHour { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
