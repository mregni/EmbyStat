using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Genres.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.People.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Core.System.Interfaces;

namespace EmbyStat.Core.System;

public class SystemService : ISystemService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IShowRepository _showRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IHubHelper _hub;
    private readonly IConfigurationService _configurationService;
    private readonly IMediaServerService _mediaServerService;
    private readonly IFilterRepository _filterRepository;

    public SystemService(IMovieRepository movieRepository, IShowRepository showRepository,
        IStatisticsRepository statisticsRepository, IMediaServerRepository mediaServerRepository,
        IGenreRepository genreRepository, IPersonRepository personRepository, IHubHelper hub,
        IConfigurationService configurationService, IMediaServerService mediaServerService, IFilterRepository filterRepository)
    {
        _movieRepository = movieRepository;
        _showRepository = showRepository;
        _statisticsRepository = statisticsRepository;
        _mediaServerRepository = mediaServerRepository;
        _genreRepository = genreRepository;
        _personRepository = personRepository;
        _hub = hub;
        _configurationService = configurationService;
        _mediaServerService = mediaServerService;
        _filterRepository = filterRepository;
    }

    public async Task ResetEmbyStatTables()
    {
        await _hub.BroadcastResetLogLine("Deleting statistics");
        await _statisticsRepository.DeleteStatistics();

        await _hub.BroadcastResetLogLine("Deleting server info");
        await _mediaServerRepository.DeleteAllPlugins();
        await _mediaServerRepository.DeleteAllDevices();
        await _mediaServerRepository.DeleteServerInfo();
        await _mediaServerRepository.DeleteAllUsers();
        await _mediaServerRepository.DeleteAllLibraries();
        
        await _hub.BroadcastResetLogLine("Deleting show data");
        await _showRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting movie data");
        await _movieRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting genres");
        await _genreRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting people");
        await _personRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting filters");
        await _filterRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Testing new media server info");
        var config = _configurationService.Get();
        var url = config.UserConfig.MediaServer.Address;
        var apiKey = config.UserConfig.MediaServer.ApiKey;
        var type = config.UserConfig.MediaServer.Type;
        var result = await _mediaServerService.TestNewApiKey(url, apiKey, type);
        if (result)
        {
            await _mediaServerService.GetAndProcessLibraries();
            await _hub.BroadcastResetLogLine("Setting new libraries");
        }

        await _hub.BroadcastResetFinished();
    }
}