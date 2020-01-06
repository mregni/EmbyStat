using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Tvdb
{
    public interface ITvdbClient
    {
        Task Login(string apiKey);
        Task<IEnumerable<VirtualEpisode>> GetEpisodes(string seriesId);
        Task<List<string>> GetShowsToUpdate(DateTime? lastUpdateTime, CancellationToken cancellationToken);
    }
}
