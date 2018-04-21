using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Movie
{
    public class PosterViewModel
    {
        public string MediaId { get; set; }
        public string Name { get; set; }
        public string CommunityRating { get; set; }
        public string OfficialRating { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public double DurationMinutes { get; set; }
        public int Year { get; set; }
    }
}
