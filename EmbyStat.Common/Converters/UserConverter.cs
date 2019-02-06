using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class UserConverter
    {
        public static IEnumerable<User> ConvertToUserList(JArray model)
        {
            foreach (var user in model.Children())
            {
                var newUser = new User
                {
                    AuthenticationProviderId =
                        user["Policy"]["AuthenticationProviderId"]?.Value<string>() ?? string.Empty,
                    DisablePremiumFeatures = user["Policy"]["DisablePremiumFeatures"].Value<bool>(),
                    DisplayCollectionsView = user["Configuration"]["DisplayCollectionsView"].Value<bool>(),
                    DisplayMissingEpisodes = user["Configuration"]["DisplayMissingEpisodes"].Value<bool>(),
                    EnableAllChannels = user["Policy"]["EnableAllChannels"].Value<bool>(),
                    EnableAllDevices = user["Policy"]["EnableAllDevices"].Value<bool>(),
                    EnableAllFolders = user["Policy"]["EnableAllFolders"].Value<bool>(),
                    EnableAudioPlaybackTranscoding = user["Policy"]["EnableAudioPlaybackTranscoding"].Value<bool>(),
                    EnableContentDeletion = user["Policy"]["EnableContentDeletion"].Value<bool>(),
                    EnableContentDownloading = user["Policy"]["EnableContentDownloading"].Value<bool>(),
                    EnableLiveTvAccess = user["Policy"]["EnableLiveTvAccess"].Value<bool>(),
                    EnableLiveTvManagement = user["Policy"]["EnableLiveTvManagement"].Value<bool>(),
                    EnableLocalPassword = user["Configuration"]["EnableLocalPassword"].Value<bool>(),
                    EnableMediaConversion = user["Policy"]["EnableMediaConversion"].Value<bool>(),
                    EnableMediaPlayback = user["Policy"]["EnableMediaPlayback"].Value<bool>(),
                    EnableNextEpisodeAutoPlay = user["Configuration"]["EnableNextEpisodeAutoPlay"].Value<bool>(),
                    EnablePlaybackRemuxing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>(),
                    EnablePublicSharing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>(),
                    EnableRemoteAccess = user["Policy"]["EnableRemoteAccess"].Value<bool>(),
                    EnableRemoteControlOfOtherUsers = user["Policy"]["EnableRemoteControlOfOtherUsers"].Value<bool>(),
                    EnableSharedDeviceControl = user["Policy"]["EnableSharedDeviceControl"].Value<bool>(),
                    EnableSubtitleDownloading = user["Policy"]["EnableSubtitleDownloading"].Value<bool>(),
                    EnableSubtitleManagement = user["Policy"]["EnableSubtitleManagement"].Value<bool>(),
                    EnableSyncTranscoding = user["Policy"]["EnableSyncTranscoding"].Value<bool>(),
                    EnableUserPreferenceAccess = user["Policy"]["EnableUserPreferenceAccess"].Value<bool>(),
                    EnableVideoPlaybackTranscoding = user["Policy"]["EnableVideoPlaybackTranscoding"].Value<bool>(),
                    HasConfiguredEasyPassword = user["HasConfiguredEasyPassword"].Value<bool>(),
                    HasConfiguredPassword = user["HasConfiguredPassword"].Value<bool>(),
                    HasPassword = user["HasPassword"].Value<bool>(),
                    HidePlayedInLatest = user["Configuration"]["HidePlayedInLatest"].Value<bool>(),
                    IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>(),
                    IsDisabled = user["Policy"]["IsDisabled"].Value<bool>(),
                    IsHidden = user["Policy"]["IsHidden"].Value<bool>(),
                    PlayDefaultAudioTrack = user["Configuration"]["PlayDefaultAudioTrack"].Value<bool>(),
                    RememberAudioSelections = user["Configuration"]["RememberAudioSelections"].Value<bool>(),
                    RememberSubtitleSelections = user["Configuration"]["RememberSubtitleSelections"].Value<bool>(),
                    InvalidLoginAttemptCount = user["Policy"]["InvalidLoginAttemptCount"].Value<long>(),
                    MaxParentalRating = user["Policy"]["MaxParentalRating"]?.Value<long?>() ?? null,
                    RemoteClientBitRateLimit = user["Policy"]["RemoteClientBitrateLimit"].Value<long>(),
                    Id = user["Id"].Value<string>(),
                    PrimaryImageTag = user["PrimaryImageTag"].Value<string>(),
                    Name = user["Name"].Value<string>(),
                    ServerId = user["ServerId"].Value<string>(),
                    SubtitleMode = user["Configuration"]["SubtitleMode"].Value<string>(),
                    LastActivityDate = user["LastActivityDate"] != null
                        ? new DateTimeOffset(user["LastActivityDate"].Value<DateTime>())
                        : (DateTimeOffset?) null,
                    LastLoginDate = user["LastLoginDate"] != null
                        ? new DateTimeOffset(user["LastLoginDate"].Value<DateTime>())
                        : (DateTimeOffset?) null,
                    ExcludedSubFolders = user["Policy"]["ExcludedSubFolders"]?.Values<string>().ToList() ??
                                         new List<string>(),
                    BlockUnratedItems = user["Policy"]["BlockUnratedItems"]?.Values<string>().ToList() ??
                                        new List<string>(),
                    BlockedTags = user["Policy"]["BlockedTags"]?.Values<string>().ToList() ?? new List<string>(),
                    EnabledChannels =
                        user["Policy"]["EnabledChannels"]?.Values<string>().ToList() ?? new List<string>(),
                    EnabledDevices = user["Policy"]["EnabledDevices"]?.Values<string>().ToList() ?? new List<string>(),
                    EnabledFolders = user["Policy"]["EnabledFolders"]?.Values<string>().ToList() ?? new List<string>(),
                    AccessSchedules = user["Policy"]["AccessSchedules"]?.Select(x => new UserAccessSchedule
                    {
                        DayOfWeek = x["DayOfWeek"].Value<string>(),
                        StartHour = x["StartHour"].Value<long>(),
                        EndHour = x["EndHour"].Value<long>()
                    }).ToList() ?? new List<UserAccessSchedule>(),
                    Deleted = false
                };
                
                yield return newUser;
            }
        }
    }
}


//yield return new User
//                {
//                    AuthenticationProviderId = user["Policy"]["AuthenticationProviderId"].Value<string>();
//                    DisablePremiumFeatures = user["Policy"]["DisablePremiumFeatures"].Value<bool>();
//                    DisplayCollectionsView = user["Configuration"]["DisplayCollectionsView"].Value<bool>();
//                    DisplayMissingEpisodes = user["Configuration"]["DisplayMissingEpisodes"].Value<bool>();
//                    EnableAllChannels = user["Policy"]["EnableAllChannels"].Value<bool>();
//                    EnableAllDevices = user["Policy"]["EnableAllDevices"].Value<bool>();
//                    EnableAllFolders = user["Policy"]["EnableAllFolders"].Value<bool>();
//                    EnableAudioPlaybackTranscoding = user["Policy"]["EnableAudioPlaybackTranscoding"].Value<bool>();
//                    EnableContentDeletion = user["Policy"]["EnableContentDeletion"].Value<bool>();
//                    EnableContentDownloading = user["Policy"]["EnableContentDownloading"].Value<bool>();
//                    EnableLiveTvAccess = user["Policy"]["EnableLiveTvAccess"].Value<bool>();
//                    EnableLiveTvManagement = user["Policy"]["EnableLiveTvManagement"].Value<bool>();
//                    EnableLocalPassword = user["Policy"]["EnableLocalPassword"].Value<bool>();
//                    EnableMediaConversion = user["Policy"]["EnableMediaConversion"].Value<bool>();
//                    EnableMediaPlayback = user["Policy"]["EnableMediaPlayback"].Value<bool>();
//                    EnableNextEpisodeAutoPlay = user["Configuration"]["EnableNextEpisodeAutoPlay"].Value<bool>();
//                    EnablePlaybackRemuxing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>();
//                    EnablePublicSharing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>();
//                    EnableRemoteAccess = user["Policy"]["EnableRemoteAccess"].Value<bool>();
//                    EnableRemoteControlOfOtherUsers = user["Policy"]["EnableRemoteControlOfOtherUsers"].Value<bool>();
//                    EnableSharedDeviceControl = user["Policy"]["EnableSharedDeviceControl"].Value<bool>();
//                    EnableSubtitleDownloading = user["Policy"]["EnableSubtitleDownloading"].Value<bool>();
//                    EnableSubtitleManagement = user["Policy"]["EnableSubtitleManagement"].Value<bool>();
//                    EnableSyncTranscoding = user["Policy"]["EnableSyncTranscoding"].Value<bool>();
//                    EnableUserPreferenceAccess = user["Policy"]["EnableUserPreferenceAccess"].Value<bool>();
//                    EnableVideoPlaybackTranscoding = user["Policy"]["EnableVideoPlaybackTranscoding"].Value<bool>();
//                    HasConfiguredEasyPassword = user["HasConfiguredEasyPassword"].Value<bool>();
//                    HasConfiguredPassword = user["HasConfiguredPassword"].Value<bool>();
//                    HasPassword = user["HasPassword"].Value<bool>();
//                    HidePlayedInLatest = user["Configuration"]["HidePlayedInLatest"].Value<bool>();
//                    IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>();
//                    IsDisabled = user["Policy"]["IsDisabled"].Value<bool>();
//                    IsHidden = user["Policy"]["IsHidden"].Value<bool>();
//                    PlayDefaultAudioTrack = user["Configuration"]["PlayDefaultAudioTrack"].Value<bool>();
//                    RememberAudioSelections = user["Configuration"]["RememberAudioSelections"].Value<bool>();
//                    RememberSubtitleSelections = user["Configuration"]["RememberSubtitleSelections"].Value<bool>();
//                    InvalidLoginAttemptCount = user["Policy"]["InvalidLoginAttemptCount"].Value<long>();
//                    MaxParentalRating = user["Policy"]["MaxParentalRating"].Value<long>();
//                    RemoteClientBitRateLimit = user["Policy"]["RemoteClientBitRateLimit"].Value<long>();
//                    Id = user["Id"].Value<string>();
//                    Name = user["Name"].Value<string>();
//                    ServerId = user["ServerId"].Value<string>();
//                    SubtitleMode = user["Configuration"]["SubtitleMode"].Value<string>();
//                    LastActivityDate = user["LastActivityDate"].Value<DateTimeOffset>();
//                    LastLoginDate = user["LastLoginDate"].Value<DateTimeOffset>();
//                    ExcludedSubFolders = user["Policy"]["ExcludedSubFolders"]?.Values<string>().ToList() ?? new List<string>();
//                    BlockUnratedItems = user["Policy"]["BlockUnratedItems"]?.Values<string>().ToList() ?? new List<string>();
//                    BlockedTags = user["Policy"]["BlockedTags"]?.Values<string>().ToList() ?? new List<string>();
//                    EnabledChannels = user["Policy"]["EnabledChannels"]?.Values<string>().ToList() ?? new List<string>();
//                    EnabledDevices = user["Policy"]["EnabledDevices"]?.Values<string>().ToList() ?? new List<string>();
//                    EnabledFolders = user["Policy"]["EnabledFolders"]?.Values<string>().ToList() ?? new List<string>();
//                    AccessSchedules = user["Policy"]["AccessSchedules"]?.Values()
//                        .Select(x => new UserAccessSchedule
//                        {
//                            DayOfWeek = x["DayOfWeek"].Value<string>();
//                            StartHour = x["StartHour"].Value<long>();
//                            EndHour = x["EndHour"].Value<long>()
//                        }).ToList() ?? new List<UserAccessSchedule>();
//                    Deleted = false
//                };