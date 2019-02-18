using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Emby
{
    public class UserMediaView
    {
        public string Id { get; set; }
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
