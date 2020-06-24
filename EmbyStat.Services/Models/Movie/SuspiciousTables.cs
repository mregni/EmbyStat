using System.Collections.Generic;

namespace EmbyStat.Services.Models.Movie
{
    public class SuspiciousTables
    {
        public IEnumerable<ShortMovie> Shorts { get; set; }
        public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
        public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }
    }
}
