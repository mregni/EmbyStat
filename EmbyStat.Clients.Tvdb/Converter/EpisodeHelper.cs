using System;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Tvdb.Converter
{
    public static class EpisodeHelper
    {
        public static VirtualEpisode ConvertToVirtualEpisode(this Data episode)
        {
            var virtualEpisode = new VirtualEpisode
            {
                Id = episode.Id.ToString(),
                FirstAired = DateTime.Parse(episode.FirstAired),
                Name = episode.EpisodeName,
                EpisodeNumber = episode.AiredEpisodeNumber,
                SeasonNumber = episode.AiredSeason
            };

            if (episode.AiredSeason > 500)
            {
                virtualEpisode.SeasonNumber = Convert.ToInt32(Math.Floor(episode.DvdSeason ?? episode.AiredSeason));
            }
            
            return virtualEpisode;
        }
    }
}