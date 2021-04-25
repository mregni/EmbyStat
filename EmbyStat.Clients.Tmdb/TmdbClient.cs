using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.Tmdb.Converter;
using EmbyStat.Common;
using EmbyStat.Common.Models.Show;
using TMDbLib.Client;
using TMDbLib.Objects.TvShows;

namespace EmbyStat.Clients.Tmdb
{
    public class TmdbClient : ITmdbClient
    {
        public async Task<IEnumerable<VirtualEpisode>> GetEpisodesAsync(int? tmdbShowId)
        {
            var episodes = new List<VirtualEpisode>();
            if (!tmdbShowId.HasValue)
            {
                return episodes;
            }
            
            var client = new TMDbClient("0ad9610e613fdbf0d62e71c96d903e0c");
            
            var show = await client.GetTvShowAsync(tmdbShowId.Value);
            foreach (var tmdbSeason in show.Seasons)
            {
                var season = await client.GetTvSeasonAsync(tmdbShowId.Value, tmdbSeason.SeasonNumber);
                episodes.AddRange(season.Episodes.Select(x => x.ConvertToVirtualEpisode()));
            }

            return episodes;
        }
    }
}
