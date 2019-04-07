using System;
using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.HelperClasses.Graphs;

namespace EmbyStat.Controllers.Emby
{
    public class EmbyUserFullViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ServerId { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? LastActivityDate { get; set; }
        public string SubtitleMode { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public long? MaxParentalRating { get; set; }
        public IList<UserAccessScheduleViewModel> AccessSchedules { get; set; }
        public long InvalidLoginAttemptCount { get; set; }
        public long RemoteClientBitRateLimit { get; set; }
        public bool Deleted { get; set; }
        public string PrimaryImageTag { get; set; }
        public CardViewModel<int> ViewedMovieCount { get; set; }
        public CardViewModel<int> ViewedEpisodeCount { get; set; }
        public IList<UserMediaViewViewModel> LastWatchedMedia { get; set; }
        public BarGraphViewModel<double> HoursPerDayGraph { get; set; } 
    }

    public class UserAccessScheduleViewModel
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; }
        public long StartHour { get; set; }
        public long EndHour { get; set; }
    }
}
