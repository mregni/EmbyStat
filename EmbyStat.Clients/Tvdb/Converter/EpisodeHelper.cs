using System;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Clients.Tvdb.Converter
{
    public static class EpisodeHelper
    {
        public static VirtualEpisode ConvertToEpisode(Data episode)
        {
            return new VirtualEpisode
            {
                EpisodeIndex = episode.AiredEpisodeNumber,
                SeasonIndex = episode.AiredSeason,
                FirstAired = Convert.ToDateTime(episode.FirstAired)
            };
        }
    }
}