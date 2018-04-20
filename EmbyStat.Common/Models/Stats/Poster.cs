using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Stats
{
    public class Poster
    {
        public string MediaId { get; set; }
        public string Name { get; set; }
        public string CommunityRating { get; set; }
        public string OfficialRating { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public double DurationMinutes { get; set; }
    }
}
