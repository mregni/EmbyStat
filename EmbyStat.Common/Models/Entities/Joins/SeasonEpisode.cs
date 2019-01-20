using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Joins
{
    public class SeasonEpisode
    {
        [Key]
        public Guid Id { get; set; }
        public string SeasonId { get; set; }
        public Season Season { get; set; }
        public string EpisodeId { get; set; }
        public Episode Episode { get; set; }
    }
}
