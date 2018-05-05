using System.Collections.Generic;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models.Helpers
{
    public class Extra : Media
    {
        public float? CommunityRating { get; set; }
        public string Overview { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public ICollection<ExtraPerson> ExtraPersons { get; set; }
    }
}
