using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Tmdb
{
    public interface ITmdbClient
    {
        Task<IEnumerable<VirtualEpisode>> GetEpisodesAsync(int? tmdbShowId);
    }
}
