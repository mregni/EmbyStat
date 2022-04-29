using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Show;
using EmbyStat.Services.Interfaces;
using TMDbLib.Client;

namespace EmbyStat.Clients.Tmdb;

public class TmdbClient : ITmdbClient
{
    private readonly ISettingsService _settingsService;
    private readonly IMapper _mapper;
    public TmdbClient(ISettingsService settingsService, IMapper mapper)
    {
        _settingsService = settingsService;
        _mapper = mapper;
    }
    public async Task<IEnumerable<VirtualEpisode>> GetEpisodesAsync(int? tmdbShowId)
    {
        if (!tmdbShowId.HasValue)
        {
            return null;
        }

        var settings = _settingsService.GetUserSettings();
        var client = new TMDbClient(settings.Tmdb.ApiKey);
            
        var show = await client.GetTvShowAsync(tmdbShowId.Value);
        if (show == null)
        {
            return null;
        }

        var episodes = new List<VirtualEpisode>();
        foreach (var tmdbSeason in show.Seasons)
        {
            var season = await client.GetTvSeasonAsync(tmdbShowId.Value, tmdbSeason.SeasonNumber);
            episodes.AddRange(_mapper.Map<IList<VirtualEpisode>>(season.Episodes));
        }

        return episodes.Where(x => x.FirstAired != null && x.FirstAired < DateTime.Now);
    }
}