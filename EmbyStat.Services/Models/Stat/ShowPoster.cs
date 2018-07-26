using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Stat
{
    public class ShowPoster
    {
        public Guid MediaId { get; set; }
        public string Name { get; set; }
        public string CommunityRating { get; set; }
        public string OfficialRating { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public int Year { get; set; }
    }
}
