using System;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class Extra : Media
    {
        public float? CommunityRating { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
        public ExtraPerson[] People { get; set; }
        public string[] Genres { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
