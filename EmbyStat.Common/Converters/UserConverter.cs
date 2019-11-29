using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class UserConverter
    {
        public static List<EmbyUser> ConvertToUserList(this JArray model)
        {
            return model.Children().Select(user => new EmbyUser
            {
                DisablePremiumFeatures = user["Policy"]["DisablePremiumFeatures"].Value<bool>(),
                EnableLocalPassword = user["Configuration"]["EnableLocalPassword"].Value<bool>(),
                HasConfiguredEasyPassword = user["HasConfiguredEasyPassword"].Value<bool>(),
                HasConfiguredPassword = user["HasConfiguredPassword"].Value<bool>(),
                HasPassword = user["HasPassword"].Value<bool>(),
                IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>(),
                IsDisabled = user["Policy"]["IsDisabled"].Value<bool>(),
                IsHidden = user["Policy"]["IsHidden"].Value<bool>(),
                PlayDefaultAudioTrack = user["Configuration"]["PlayDefaultAudioTrack"].Value<bool>(),
                InvalidLoginAttemptCount = user["Policy"]["InvalidLoginAttemptCount"].Value<long>(),
                MaxParentalRating = user["Policy"]["MaxParentalRating"]?.Value<long?>() ?? null,
                RemoteClientBitRateLimit = user["Policy"]["RemoteClientBitrateLimit"].Value<long>(),
                Id = user["Id"].Value<string>(),
                PrimaryImageTag = user["PrimaryImageTag"]?.Value<string>() ?? string.Empty,
                Name = user["Name"].Value<string>(),
                ServerId = user["ServerId"].Value<string>(),
                SubtitleMode = user["Configuration"]["SubtitleMode"].Value<string>(),
                LastActivityDate = user["LastActivityDate"] != null
                    ? new DateTimeOffset(user["LastActivityDate"].Value<DateTime>())
                    : (DateTimeOffset?) null,
                LastLoginDate = user["LastLoginDate"] != null
                    ? new DateTimeOffset(user["LastLoginDate"].Value<DateTime>())
                    : (DateTimeOffset?) null,
                BlockUnratedItems = user["Policy"]["BlockUnratedItems"]?.Values<string>().ToList() ??
                                    new List<string>(),
                BlockedTags = user["Policy"]["BlockedTags"]?.Values<string>().ToList() ?? new List<string>(),
                AccessSchedules = user["Policy"]["AccessSchedules"]?.Select(x => new UserAccessSchedule
                {
                    DayOfWeek = x["DayOfWeek"].Value<string>(),
                    StartHour = x["StartHour"].Value<long>(),
                    EndHour = x["EndHour"].Value<long>()
                }).ToList() ?? new List<UserAccessSchedule>(),
                Deleted = false
            }).ToList();
        }
    }
}