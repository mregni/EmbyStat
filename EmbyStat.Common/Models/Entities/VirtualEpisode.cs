using LiteDB;
using System;

namespace EmbyStat.Common.Models.Entities
{
    public class VirtualEpisode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeasonIndex { get; set; }
        public int EpisodeIndex { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
