using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Common.Models.Show;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using TMDbLib.Client;

namespace EmbyStat.Clients.Tmdb;

public class TmdbClient : ITmdbClient
{
    private readonly Config _config;
    private readonly IMapper _mapper;
    public TmdbClient(IConfigurationService configurationService, IMapper mapper)
    {
        _config = configurationService.Get();
        _mapper = mapper;
    }
    public async Task<IEnumerable<VirtualEpisode>> GetEpisodesAsync(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        var client = new TMDbClient(_config.UserConfig.Tmdb.ApiKey);
            
        var show = await client.GetTvShowAsync(id.Value);
        if (show == null)
        {
            return null;
        }

        var episodes = new List<VirtualEpisode>();
        foreach (var tmdbSeason in show.Seasons)
        {
            var season = await client.GetTvSeasonAsync(id.Value, tmdbSeason.SeasonNumber);
            episodes.AddRange(_mapper.Map<IList<VirtualEpisode>>(season.Episodes));
        }

        return episodes.Where(x => x.FirstAired != null && x.FirstAired < DateTime.Now);
    }
}