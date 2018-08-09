using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Movie
{
    public class SuspiciousTables
    {
        public List<MovieDuplicate> Duplicates { get; set; }
        public List<ShortMovie> Shorts { get; set; }
        public List<SuspiciousMovie> NoImdb { get; set; }
        public List<SuspiciousMovie> NoPrimary { get; set; }
    }
}
