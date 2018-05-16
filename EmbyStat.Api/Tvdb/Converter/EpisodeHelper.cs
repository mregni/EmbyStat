using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Api.Tvdb.Models;
using EmbyStat.Common.Models;

namespace EmbyStat.Api.Tvdb.Converter
{
    public static class EpisodeHelper
    {
        public static VirtualEpisode ConvertToEpisode(Data episode)
        {
            return new VirtualEpisode
            {
                EpisodeIndex = episode.AiredEpisodeNumber,
                SeasonIndex = episode.AiredSeason
            };
        }
    }
}
