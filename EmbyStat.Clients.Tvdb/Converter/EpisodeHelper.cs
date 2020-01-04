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
                Id = episode.Id.ToString(),
                EpisodeNumber = Convert.ToInt32(Math.Floor(episode.DvdEpisodeNumber ?? episode.AiredEpisodeNumber)),
                SeasonNumber = Convert.ToInt32(Math.Floor(episode.DvdSeason ?? episode.AiredSeason)),
                FirstAired = DateTime.Parse(episode.FirstAired),
                Name = episode.EpisodeName
            };
        }
    }
}