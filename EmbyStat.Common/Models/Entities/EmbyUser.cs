using LiteDB;
using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities
{
    public class EmbyUser
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ServerId { get; set; }
        public bool HasPassword { get; set; }
        public bool HasConfiguredPassword { get; set; }
        public bool HasConfiguredEasyPassword { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool PlayDefaultAudioTrack { get; set; }
        public string SubtitleMode { get; set; }
        public bool EnableLocalPassword { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public long? MaxParentalRating { get; set; }
        public List<string> BlockedTags { get; set; }
        public IList<UserAccessSchedule> AccessSchedules { get; set; }
        public List<string> BlockUnratedItems { get; set; }
        public long InvalidLoginAttemptCount { get; set; }
        public long RemoteClientBitRateLimit { get; set; }
        public bool DisablePremiumFeatures { get; set; }
        public bool Deleted { get; set; }
        public string PrimaryImageTag { get; set; }
    }

    public class UserAccessSchedule
    {
        public string DayOfWeek { get; set; }
        public long StartHour { get; set; }
        public long EndHour { get; set; }
    }
}
