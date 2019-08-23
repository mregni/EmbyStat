using System;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Clients.Tvdb.Converter
{
    public static class EpisodeHelper
    {
        public static VirtualEpisode ConvertToVirtualEpisode(this Data episode)
        {
            return new VirtualEpisode
            {
                Id = episode.Id,
                EpisodeIndex = episode.AiredEpisodeNumber,
                SeasonIndex = episode.AiredSeason,
                FirstAired = Convert.ToDateTime(episode.FirstAired),
                Name = episode.EpisodeName
            };
        }
    }
}