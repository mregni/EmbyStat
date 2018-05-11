using System.Collections.Generic;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
{
    public class Season : Media
    {
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public ICollection<SeasonEpisode> SeasonEpisodes { get; set; }
    }
}
