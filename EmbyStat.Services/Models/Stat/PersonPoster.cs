using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonPoster
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ChildCount { get; set; }
        public int? MovieCount { get; set; }
        public int? EpisodeCount { get; set; }
        public bool HasTitle { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
    }
}
