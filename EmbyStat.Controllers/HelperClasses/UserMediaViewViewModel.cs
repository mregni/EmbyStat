using System;

namespace EmbyStat.Controllers.HelperClasses
{
    public class UserMediaViewViewModel
    {
        public int Id { get; set; }
        public string Primary { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public decimal? WatchedPercentage { get; set; }
        public double WatchedTime { get; set; }
        public DateTimeOffset StartedWatching { get; set; }
        public DateTimeOffset EndedWatching { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLogo { get; set; }
    }
}
