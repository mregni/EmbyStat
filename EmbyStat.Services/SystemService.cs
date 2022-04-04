using System.Threading.Tasks;
using EmbyStat.Common.Hubs;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services;

public class SystemService : ISystemService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IShowRepository _showRepository;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IHubHelper _hub;
    private readonly ISettingsService _settingsService;
    private readonly IMediaServerService _mediaServerService;
    private readonly IFilterRepository _filterRepository;

    public SystemService(IMovieRepository movieRepository, IShowRepository showRepository,
        IStatisticsRepository statisticsRepository, IMediaServerRepository mediaServerRepository,
        IGenreRepository genreRepository, IPersonRepository personRepository, IHubHelper hub,
        ISettingsService settingsService, IMediaServerService mediaServerService, IFilterRepository filterRepository)
    {
        _movieRepository = movieRepository;
        _showRepository = showRepository;
        _statisticsRepository = statisticsRepository;
        _mediaServerRepository = mediaServerRepository;
        _genreRepository = genreRepository;
        _personRepository = personRepository;
        _hub = hub;
        _settingsService = settingsService;
        _mediaServerService = mediaServerService;
        _filterRepository = filterRepository;
    }

    public async Task ResetEmbyStatTables()
    {
        await _hub.BroadcastResetLogLine("Deleting statistics");
        _statisticsRepository.DeleteStatistics();

        await _hub.BroadcastResetLogLine("Deleting server info");
        await _mediaServerRepository.DeleteAllPlugins();
        await _mediaServerRepository.DeleteAllDevices();
        await _mediaServerRepository.DeleteAllUsers();
        await _mediaServerRepository.DeleteServerInfo();

        await _hub.BroadcastResetLogLine("Deleting show data");
        await _showRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting movie data");
        await _movieRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting genres");
        await _genreRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting people");
        await _personRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Deleting libraries");
        await _mediaServerRepository.DeleteAllLibraries();
        
        await _hub.BroadcastResetLogLine("Deleting devices");
        await _mediaServerRepository.DeleteAllDevices();
        
        await _hub.BroadcastResetLogLine("Deleting plugins");
        await _mediaServerRepository.DeleteAllPlugins();
        
        await _hub.BroadcastResetLogLine("Deleting users");
        await _mediaServerRepository.DeleteAllUsers();

        await _hub.BroadcastResetLogLine("Deleting filters");
        await _filterRepository.DeleteAll();

        await _hub.BroadcastResetLogLine("Testing new media server info");
        var userSettings = _settingsService.GetUserSettings();
        var url = userSettings.MediaServer.Address;
        var apiKey = userSettings.MediaServer.ApiKey;
        var type = userSettings.MediaServer.Type;
        var result = await _mediaServerService.TestNewApiKey(url, apiKey, type);
        if (result)
        {
            await _mediaServerService.GetAndProcessLibraries();
            await _hub.BroadcastResetLogLine("Setting new libraries");
        }

        await _hub.BroadcastResetFinished();
    }
}