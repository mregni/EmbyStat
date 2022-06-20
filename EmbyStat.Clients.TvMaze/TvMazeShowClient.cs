using AutoMapper;
using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Clients.Tmdb;
using EmbyStat.Common.Models.Show;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using TvMaze.Api.Client;
using TvMaze.Api.Client.Models;

namespace EmbyStat.Clients.TvMaze;

public class TvMazeShowClient : ITvMazeShowClient
{
    private readonly IMapper _mapper;

    public TvMazeShowClient(IConfigurationService configurationService, IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<VirtualEpisode>?> GetEpisodesAsync(int? tmdbShowId)
    {
        if (!tmdbShowId.HasValue)
        {
            return null;
        }

        var client = new TvMazeClient();
        var show = await client.Lookup.GetShowByTheTvdbIdAsync(tmdbShowId.Value);

        if (show == null)
        {
            return null;
        }

        var episodes = await client.Shows.GetShowEpisodeListAsync(show.Id);
        return episodes?
            .Where(x => x.Type == EpisodeType.Regular)
            .Select(x => _mapper.Map<VirtualEpisode>(x))
            .Where(x => x.FirstAired != null && x.FirstAired < DateTime.Now);

    }
}