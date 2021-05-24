using System;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Models.Show
{
    public class VirtualEpisode
    {
        public VirtualEpisode()
        {
            
        }

        public VirtualEpisode(Episode episode)
        {
            Id = episode.Id;
            Name = episode.Name;
            SeasonNumber = episode?.SeasonIndexNumber ?? 0;
            EpisodeNumber = episode.IndexNumber ?? 0;
            FirstAired = episode.PremiereDate;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime? FirstAired { get; set; }
    }
}
