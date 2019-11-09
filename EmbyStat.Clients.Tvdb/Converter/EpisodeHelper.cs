using System;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Tvdb.Converter
{
    public static class EpisodeHelper
    {
        public static VirtualEpisode ConvertToVirtualEpisode(this Data episode)
        {
            return new VirtualEpisode
            {
                Id = episode.Id,
                EpisodeNumber = episode.AiredEpisodeNumber,
                SeasonNumber = episode.AiredSeason,
                FirstAired = Convert.ToDateTime(episode.FirstAired),
                Name = episode.EpisodeName
            };
        }
    }
}