using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
{
    public class Episode: Video
    {
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
	    public int? IndexNumber { get; set; }
	    public int? IndexNumberEnd { get; set; }
        public ICollection<SeasonEpisode> SeasonEpisodes { get; set; }
    }
}
