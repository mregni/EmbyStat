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
            return model.Children().Select(user =>
            {
                var embyUser = new EmbyUser();
                embyUser.DisablePremiumFeatures = user["Policy"]["DisablePremiumFeatures"]?.Value<bool>() ?? false;
                embyUser.EnableLocalPassword = user["Configuration"]["EnableLocalPassword"].Value<bool>();
                embyUser.HasConfiguredEasyPassword = user["HasConfiguredEasyPassword"].Value<bool>();
                embyUser.HasConfiguredPassword = user["HasConfiguredPassword"].Value<bool>();
                embyUser.HasPassword = user["HasPassword"].Value<bool>();
                embyUser.IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>();
                embyUser.IsDisabled = user["Policy"]["IsDisabled"].Value<bool>();
                embyUser.IsHidden = user["Policy"]["IsHidden"].Value<bool>();
                embyUser.PlayDefaultAudioTrack = user["Configuration"]["PlayDefaultAudioTrack"].Value<bool>();
                embyUser.InvalidLoginAttemptCount = user["Policy"]["InvalidLoginAttemptCount"].Value<long>();
                embyUser.MaxParentalRating = user["Policy"]["MaxParentalRating"]?.Value<long?>() ?? null;
                embyUser.RemoteClientBitRateLimit = user["Policy"]["RemoteClientBitrateLimit"].Value<long>();
                embyUser.Id = user["Id"].Value<string>();
                embyUser.PrimaryImageTag = user["PrimaryImageTag"]?.Value<string>() ?? string.Empty;
                embyUser.Name = user["Name"].Value<string>();
                embyUser.ServerId = user["ServerId"].Value<string>();
                embyUser.SubtitleMode = user["Configuration"]["SubtitleMode"].Value<string>();
                embyUser.LastActivityDate = user["LastActivityDate"].Value<DateTime?>();
                embyUser.LastLoginDate = user["LastLoginDate"].Value<DateTime?>();
                embyUser.BlockUnratedItems = user["Policy"]["BlockUnratedItems"]?.Values<string>().ToList() ??
                                             new List<string>();
                embyUser.BlockedTags =
                    user["Policy"]["BlockedTags"]?.Values<string>().ToList() ?? new List<string>();
                embyUser.AccessSchedules = user["Policy"]["AccessSchedules"]?.Select(x => new UserAccessSchedule
                {
                    DayOfWeek = x["DayOfWeek"].Value<string>(),
                    StartHour = x["StartHour"].Value<long>(),
                    EndHour = x["EndHour"].Value<long>()
                }).ToList() ?? new List<UserAccessSchedule>();
                embyUser.Deleted = false;
                return embyUser;
            }).ToList();
        }
    }
}