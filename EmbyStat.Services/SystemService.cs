using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Settings;
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

    public SystemService(IMovieRepository movieRepository, IShowRepository showRepository,
        IStatisticsRepository statisticsRepository, IMediaServerRepository mediaServerRepository,
        IGenreRepository genreRepository, IPersonRepository personRepository, IHubHelper hub,
        ISettingsService settingsService, IMediaServerService mediaServerService)
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

        var userSettings = _settingsService.GetUserSettings();
        userSettings.MovieLibraries = new List<LibraryContainer>();
        userSettings.ShowLibraries = new List<LibraryContainer>();

        var url = userSettings.MediaServer.Address;
        var apiKey = userSettings.MediaServer.ApiKey;
        var type = userSettings.MediaServer.Type;
        await _hub.BroadcastResetLogLine("Testing new media server info");

        var result = _mediaServerService.TestNewApiKey(url, apiKey, type);
        if (result)
        {
            var libraries = _mediaServerService.GetMediaServerLibraries().ToList();
            userSettings.MovieLibraries = libraries
                .Where(x => x.Type == LibraryType.Movies)
                .Select(x => new LibraryContainer { Id = x.Id, Name = x.Name})
                .ToList();
            userSettings.ShowLibraries = libraries
                .Where(x => x.Type == LibraryType.TvShow)
                .Select(x => new LibraryContainer { Id = x.Id, Name = x.Name})
                .ToList();
            await _hub.BroadcastResetLogLine("Setting new libraries");

        }
        await _settingsService.SaveUserSettingsAsync(userSettings);
        await _hub.BroadcastResetLogLine("Saving new configuration");

        await _hub.BroadcastResetFinished();
    }
}