﻿using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.MediaServer;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Abstract;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Sessions.Interfaces;
using EmbyStat.Core.Shows;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Core.MediaServers;

public class MediaServerService : StatisticHelper, IMediaServerService
{
    private IBaseHttpClient _baseHttpClient;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly ISessionService _sessionService;
    private readonly IConfigurationService _configurationService;
    private readonly IClientStrategy _clientStrategy;
    private readonly ILogger<MediaServerService> _logger;
    private readonly IStatisticsRepository _statisticsRepository;

    public MediaServerService(IClientStrategy clientStrategy, IMediaServerRepository mediaServerRepository,
        ISessionService sessionService, IConfigurationService configurationService, ILogger<MediaServerService> logger,
        IStatisticsRepository statisticsRepository, IJobRepository jobRepository) : base(logger, jobRepository)
    {
        _mediaServerRepository = mediaServerRepository;
        _sessionService = sessionService;
        _configurationService = configurationService;
        _clientStrategy = clientStrategy;
        _logger = logger;
        _statisticsRepository = statisticsRepository;

        var config = _configurationService.Get();
        ChangeClientType(config.UserConfig.MediaServer.Type);
    }

    #region Server

    public Task<IEnumerable<MediaServerUdpBroadcast>> SearchMediaServer(ServerType type)
    {
        ChangeClientType(type);
        return _baseHttpClient.SearchServer();
    }

    public async Task<MediaServerInfo> GetServerInfo(bool forceReSync)
    {
        if (forceReSync)
        {
            return await GetAndProcessServerInfo();
        }

        return await _mediaServerRepository.GetServerInfo() ?? await GetAndProcessServerInfo();
    }

    public async Task<bool> TestNewApiKey(string url, string apiKey, ServerType type)
    {
        _logger.LogDebug($"Testing new API key on {url}");
        _logger.LogDebug($"API key used: {apiKey}");
        ChangeClientType(type);
        var oldApiKey = _baseHttpClient.ApiKey;
        var oldUrl = _baseHttpClient.BaseUrl;
        _baseHttpClient.ApiKey = apiKey;
        _baseHttpClient.BaseUrl = url;

        var info = await _baseHttpClient.GetServerInfo();
        if (info != null)
        {
            _logger.LogDebug("new API key works!");
            return true;
        }

        _baseHttpClient.ApiKey = oldApiKey;
        _baseHttpClient.BaseUrl = oldUrl;

        _logger.LogDebug("new API key not working!");
        return false;
    }

    public Task<MediaServerStatus> GetMediaServerStatus()
    {
        return _mediaServerRepository.GetEmbyStatus();
    }

    public async Task<Library[]> GetMediaServerLibraries()
    {
        var items = await _baseHttpClient.GetLibraries();
        var libraries = items
            .Where(x => x.Type != LibraryType.BoxSets)
            .ToArray();

        await _mediaServerRepository.DeleteAndInsertLibraries(libraries);
        return libraries;
    }

    public async Task<bool> PingMediaServer(string url)
    {
        _logger.LogDebug($"Pinging server on {url}");
        var oldUrl = _baseHttpClient.BaseUrl;
        _baseHttpClient.BaseUrl = url;

        var result = await _baseHttpClient.Ping();

        _baseHttpClient.BaseUrl = oldUrl;
        return result;
    }

    public async Task<bool> PingMediaServer()
    {
        var address = _configurationService.Get().UserConfig.MediaServer.Address;
        _logger.LogDebug($"Pinging server on {address}");
        return await _baseHttpClient.Ping();
    }

    public Task ResetMissedPings()
    {
        return _mediaServerRepository.ResetMissedPings();
    }

    public Task IncreaseMissedPings()
    {
        return _mediaServerRepository.IncreaseMissedPings();
    }

    #endregion

    #region Plugin

    public Task<List<PluginInfo>> GetAllPlugins()
    {
        return _mediaServerRepository.GetAllPlugins();
    }

    #endregion

    #region Users

    #region Statistics

    public async Task<MediaServerUserStatistics> GetMediaServerUserStatistics()
    {
        var statistic = await _statisticsRepository.GetLastResultByType(StatisticType.User);

        MediaServerUserStatistics statistics;
        if (StatisticsAreValid(statistic, Constants.JobIds.SmallSyncId))
        {
            statistics = JsonConvert.DeserializeObject<MediaServerUserStatistics>(statistic.JsonResult);
        }
        else
        {
            statistics = await CalculateMediaServerUserStatistics();
        }

        return statistics;
    }

    public async Task<MediaServerUserStatistics> CalculateMediaServerUserStatistics()
    {
        var statistics = new MediaServerUserStatistics
        {
            Cards = await CalculateCards()
        };

        var json = JsonConvert.SerializeObject(statistics);
        await _statisticsRepository.ReplaceStatistic(json, DateTime.UtcNow, StatisticType.User);

        return statistics;
    }

    #region Cards

    private async Task<List<Card>> CalculateCards()
    {
        var list = new List<Card>();
        list.AddIfNotNull(await CalculateActiveUsers());
        list.AddIfNotNull(await CalculateIdleUsers());
        return list;
    }


    private Task<Card> CalculateActiveUsers()
    {
        return CalculateStat(async () =>
        {
            var users = await _mediaServerRepository.GetAllUsers();
            var count = users.Count(x => x.LastActivityDate > DateTime.Now.AddMonths(-6));
            return new Card
            {
                Title = Constants.MediaServer.TotalActiveUsers,
                Value = count.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }, "Calculate total active user count failed:");
    }

    private Task<Card> CalculateIdleUsers()
    {
        return CalculateStat(async () =>
        {
            var users = await _mediaServerRepository.GetAllUsers();
            var count = users.Count(x => x.LastActivityDate <= DateTime.Now.AddMonths(-6));
            return new Card
            {
                Title = Constants.MediaServer.TotalIdleUsers,
                Value = count.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }, "Calculate total idle user count failed:");
    }

    #endregion

    #endregion

    public async Task<Page<MediaServerUserRow>> GetUserPage(int skip, int take, string sortField, string sortOrder,
        bool requireTotalCount)
    {
        var list = await _mediaServerRepository.GetUserPage(skip, take, sortField, sortOrder);
        var page = new Page<MediaServerUserRow>(list);

        if (requireTotalCount)
        {
            page.TotalCount = await _mediaServerRepository.GetUserCount();
        }

        return page;
    }

    public Task<MediaServerUser[]> GetAllUsers()
    {
        return _mediaServerRepository.GetAllUsers();
    }

    public async Task<MediaServerUser[]> GetAllAdministrators()
    {
        var administrators = await _mediaServerRepository.GetAllAdministrators();

        if (administrators.Any())
        {
            return administrators;
        }

        await GetAndProcessUsers();
        return await _mediaServerRepository.GetAllAdministrators();
    }

    public Task<MediaServerUser?> GetUserById(string id)
    {
        return _mediaServerRepository.GetUserById(id);
    }

    public async Task<Card> GetViewedEpisodeCountByUserId(string id)
    {
        var count = await _mediaServerRepository.GetMediaServerViewsForUser(id, MediaType.Episode);
        return new Card
        {
            Title = Constants.Users.TotalWatchedEpisodes,
            Value = count.ToString()
        };
    }

    public async Task<Card> GetViewedMovieCountByUserId(string id)
    {
        var count = await _mediaServerRepository.GetMediaServerViewsForUser(id, MediaType.Movie);
        return new Card
        {
            Title = Constants.Users.TotalWatchedMovies,
            Value = count.ToString()
        };
    }

    public IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size)
    {
        throw new NotImplementedException();
        // var skip = page * size;
        // var sessions = _sessionService.GetSessionsForUser(id).ToList();
        // var plays = sessions.SelectMany(x => x.Plays).Skip(skip).Take(size).ToList();
        // var deviceIds = sessions.Where(x => plays.Any(y => x.Plays.Any(z => z.Id == y.Id))).Select(x => x.DeviceId);
        //
        // var devices = await _mediaServerRepository.GetDeviceById(deviceIds);
        //
        // var list = new List<UserMediaView>();
        // foreach (var play in plays)
        // {
        //     var currentSession = sessions.Single(x => x.Plays.Any(y => y.Id == play.Id));
        //     var device = devices.SingleOrDefault(x => x.Id == currentSession.DeviceId);
        //
        //     if (play.Type == PlayType.Movie)
        //     {
        //         list(CreateUserMediaViewFromMovie(play, device));
        //     }
        //     else if (play.Type == PlayType.Episode)
        //     {
        //         yield return CreateUserMediaViewFromEpisode(play, device);
        //     }
        //
        //     //if media is null this means a user has watched something that is not yet in our DB
        //     //TODO: try starting a sync here
        // }
    }

    #endregion

    #region Devices

    public Task<List<Device>> GetAllDevices()
    {
        return _mediaServerRepository.GetAllDevices();
    }

    #endregion

    #region JobHelpers

    public async Task<MediaServerInfo> GetAndProcessServerInfo()
    {
        var server = await _baseHttpClient.GetServerInfo();
        await _mediaServerRepository.DeleteAndInsertServerInfo(server);
        return server;
    }

    public async Task GetAndProcessPluginInfo()
    {
        var plugins = await _baseHttpClient.GetInstalledPlugins();
        await _mediaServerRepository.DeleteAllPlugins();
        await _mediaServerRepository.InsertPlugins(plugins);
    }

    public async Task GetAndProcessUsers()
    {
        var users = await _baseHttpClient.GetUsers();
        await _mediaServerRepository.DeleteAndInsertUsers(users);
    }

    public async Task<int> ProcessViewsForUser(string id)
    {
        var viewCount = await _baseHttpClient.GetPlayedMediaCountForUser(id);
        var processed = 0;
        const int limit = 500;
        var totalViews = new List<MediaServerUserView>();
        do
        {
            var views = await _baseHttpClient.GetPlayedMediaForUser(id, processed, limit);
            totalViews.AddRange(views);
            processed += limit;
        } while (processed < viewCount);

        await _mediaServerRepository.InsertOrUpdateUserViews(totalViews);
        return viewCount;
    }

    public async Task GetAndProcessDevices()
    {
        var devices = await _baseHttpClient.GetDevices();
        await _mediaServerRepository.DeleteAndInsertDevices(devices);
    }

    public async Task GetAndProcessLibraries()
    {
        var libraries = await _baseHttpClient.GetLibraries();
        var currentLibraries = await _mediaServerRepository.GetAllLibraries();

        foreach (var library in libraries)
        {
            library.SyncTypes = currentLibraries
                .FirstOrDefault(x => x.Id == library.Id)
                ?.SyncTypes ?? null;
        }

        await _mediaServerRepository.DeleteAndInsertLibraries(libraries);
    }

    #endregion

    // private UserMediaView CreateUserMediaViewFromMovie(Play play, Device device)
    // {
    //     var movie = _movieRepository.GetById(play.MediaId);
    //     if (movie == null)
    //     {
    //         throw new BusinessException("MOVIENOTFOUND");
    //     }
    //
    //     var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
    //     var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
    //     var watchedTime = endedPlaying - startedPlaying;
    //
    //     return new UserMediaView
    //     {
    //         Id = movie.Id,
    //         Name = movie.Name,
    //         Primary = movie.Primary,
    //         StartedWatching = startedPlaying,
    //         EndedWatching = endedPlaying,
    //         WatchedTime = Math.Round(watchedTime.TotalSeconds),
    //         WatchedPercentage = CalculateWatchedPercentage(play, movie),
    //         DeviceId = device?.Id ?? string.Empty,
    //         DeviceLogo = device?.IconUrl ?? string.Empty
    //     };
    // }

    private void ChangeClientType(ServerType? type)
    {
        var realType = type ?? ServerType.Emby;
        _baseHttpClient = _clientStrategy.CreateHttpClient(realType);
        var config = _configurationService.Get();

        _baseHttpClient.SetDeviceInfo(
            config.SystemConfig.AppName,
            config.UserConfig.MediaServer.AuthorizationScheme,
            config.SystemConfig.Version.ToCleanVersionString(),
            config.SystemConfig.Id.ToString(),
            config.UserConfig.MediaServer.UserId);

        config.UserConfig.MediaServer.Type = realType;
        _configurationService.UpdateUserConfiguration(config.UserConfig);
    }
}