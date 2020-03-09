using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Tvdb
{
    public interface ITvdbClient
    {
        bool Login(string apiKey);
        IEnumerable<VirtualEpisode> GetEpisodes(string seriesId);
        List<string> GetShowsToUpdate(DateTime? lastUpdateTime);
    }
}
