using System;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Models.Show
{
    public class VirtualEpisode
    {
        public VirtualEpisode()
        {
            
        }

        public VirtualEpisode(Episode episode, Season season)
        {
            Id = episode.Id;
            Name = episode.Name;
            SeasonNumber = season.IndexNumber ?? 0;
            EpisodeNumber = episode.IndexNumber ?? 0;
            FirstAired = episode.PremiereDate?.DateTime;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
