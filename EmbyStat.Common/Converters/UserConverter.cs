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
                var newUser = new User();

                newUser.AuthenticationProviderId = user["Policy"]["AuthenticationProviderId"]?.Value<string>() ?? string.Empty;
                newUser.DisablePremiumFeatures = user["Policy"]["DisablePremiumFeatures"].Value<bool>();
                newUser.DisplayCollectionsView = user["Configuration"]["DisplayCollectionsView"].Value<bool>();
                newUser.DisplayMissingEpisodes = user["Configuration"]["DisplayMissingEpisodes"].Value<bool>();
                newUser.EnableAllChannels = user["Policy"]["EnableAllChannels"].Value<bool>();
                newUser.EnableAllDevices = user["Policy"]["EnableAllDevices"].Value<bool>();
                newUser.EnableAllFolders = user["Policy"]["EnableAllFolders"].Value<bool>();
                newUser.EnableAudioPlaybackTranscoding = user["Policy"]["EnableAudioPlaybackTranscoding"].Value<bool>();
                newUser.EnableContentDeletion = user["Policy"]["EnableContentDeletion"].Value<bool>();
                newUser.EnableContentDownloading = user["Policy"]["EnableContentDownloading"].Value<bool>();
                newUser.EnableLiveTvAccess = user["Policy"]["EnableLiveTvAccess"].Value<bool>();
                newUser.EnableLiveTvManagement = user["Policy"]["EnableLiveTvManagement"].Value<bool>();
                newUser.EnableLocalPassword = user["Configuration"]["EnableLocalPassword"].Value<bool>();
                newUser.EnableMediaConversion = user["Policy"]["EnableMediaConversion"].Value<bool>();
                newUser.EnableMediaPlayback = user["Policy"]["EnableMediaPlayback"].Value<bool>();
                newUser.EnableNextEpisodeAutoPlay = user["Configuration"]["EnableNextEpisodeAutoPlay"].Value<bool>();
                newUser.EnablePlaybackRemuxing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>();
                newUser.EnablePublicSharing = user["Policy"]["EnablePlaybackRemuxing"].Value<bool>();
                newUser.EnableRemoteAccess = user["Policy"]["EnableRemoteAccess"].Value<bool>();
                newUser.EnableRemoteControlOfOtherUsers = user["Policy"]["EnableRemoteControlOfOtherUsers"].Value<bool>();
                newUser.EnableSharedDeviceControl = user["Policy"]["EnableSharedDeviceControl"].Value<bool>();
                newUser.EnableSubtitleDownloading = user["Policy"]["EnableSubtitleDownloading"].Value<bool>();
                newUser.EnableSubtitleManagement = user["Policy"]["EnableSubtitleManagement"].Value<bool>();
                newUser.EnableSyncTranscoding = user["Policy"]["EnableSyncTranscoding"].Value<bool>();
                newUser.EnableUserPreferenceAccess = user["Policy"]["EnableUserPreferenceAccess"].Value<bool>();
                newUser.EnableVideoPlaybackTranscoding = user["Policy"]["EnableVideoPlaybackTranscoding"].Value<bool>();
                newUser.HasConfiguredEasyPassword = user["HasConfiguredEasyPassword"].Value<bool>();
                newUser.HasConfiguredPassword = user["HasConfiguredPassword"].Value<bool>();
                newUser.HasPassword = user["HasPassword"].Value<bool>();
                newUser.HidePlayedInLatest = user["Configuration"]["HidePlayedInLatest"].Value<bool>();
                newUser.IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>();
                newUser.IsDisabled = user["Policy"]["IsDisabled"].Value<bool>();
                newUser.IsHidden = user["Policy"]["IsHidden"].Value<bool>();
                newUser.PlayDefaultAudioTrack = user["Configuration"]["PlayDefaultAudioTrack"].Value<bool>();
                newUser.RememberAudioSelections = user["Configuration"]["RememberAudioSelections"].Value<bool>();
                newUser.RememberSubtitleSelections = user["Configuration"]["RememberSubtitleSelections"].Value<bool>();
                newUser.InvalidLoginAttemptCount = user["Policy"]["InvalidLoginAttemptCount"].Value<long>();
                newUser.MaxParentalRating = user["Policy"]["MaxParentalRating"]?.Value<long?>() ?? null;
                newUser.RemoteClientBitRateLimit = user["Policy"]["RemoteClientBitrateLimit"].Value<long>();
                newUser.Id = user["Id"].Value<string>();
                newUser.Name = user["Name"].Value<string>();
                newUser.ServerId = user["ServerId"].Value<string>();
                newUser.SubtitleMode = user["Configuration"]["SubtitleMode"].Value<string>();
                newUser.LastActivityDate = user["LastActivityDate"] != null ? new DateTimeOffset(user["LastActivityDate"].Value<DateTime>()) : (DateTimeOffset?)null;
                newUser.LastLoginDate = user["LastLoginDate"] != null ? new DateTimeOffset(user["LastLoginDate"].Value<DateTime>()) : (DateTimeOffset?)null;
                newUser.ExcludedSubFolders = user["Policy"]["ExcludedSubFolders"]?.Values<string>().ToList() ?? new List<string>();
                newUser.BlockUnratedItems = user["Policy"]["BlockUnratedItems"]?.Values<string>().ToList() ?? new List<string>();
                newUser.BlockedTags = user["Policy"]["BlockedTags"]?.Values<string>().ToList() ?? new List<string>();
                newUser.EnabledChannels = user["Policy"]["EnabledChannels"]?.Values<string>().ToList() ?? new List<string>();
                newUser.EnabledDevices = user["Policy"]["EnabledDevices"]?.Values<string>().ToList() ?? new List<string>();
                newUser.EnabledFolders = user["Policy"]["EnabledFolders"]?.Values<string>().ToList() ?? new List<string>();
                newUser.AccessSchedules = user["Policy"]["AccessSchedules"]?.Select(x => new UserAccessSchedule
                {
                    DayOfWeek = x["DayOfWeek"].Value<string>(),
                    StartHour = x["StartHour"].Value<long>(),
                    EndHour = x["EndHour"].Value<long>()
                }).ToList() ?? new List<UserAccessSchedule>();
                newUser.Deleted = false;

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