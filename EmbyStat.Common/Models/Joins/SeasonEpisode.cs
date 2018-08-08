using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models.Joins
{
    public class SeasonEpisode
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SeasonId { get; set; }
        public Season Season { get; set; }
        public Guid EpisodeId { get; set; }
        public Episode Episode { get; set; }
    }
}
