using System.Collections.Generic;

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
        public List<ExtraPerson> People { get; set; }
        public List<string> GenresIds { get; set; }

    }
}
