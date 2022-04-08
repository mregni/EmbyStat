using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Logging;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services
{
    public class MediaServerService : IMediaServerService
    {
        private IBaseHttpClient _baseHttpClient;
        private readonly IMediaServerRepository _mediaServerRepository;
        private readonly ISessionService _sessionService;
        private readonly ISettingsService _settingsService;
        private readonly IMovieRepository _movieRepository;
        private readonly IClientStrategy _clientStrategy;
        private readonly Logger _logger;
        private readonly IMapper _mapper;

        public MediaServerService(IClientStrategy clientStrategy, IMediaServerRepository mediaServerRepository,
            ISessionService sessionService,
            ISettingsService settingsService, IMovieRepository movieRepository, IMapper mapper)
        {
            _mediaServerRepository = mediaServerRepository;
            _sessionService = sessionService;
            _settingsService = settingsService;
            _movieRepository = movieRepository;
            _clientStrategy = clientStrategy;
            _mapper = mapper;
            _logger = LogFactory.CreateLoggerForType(typeof(MediaServerService), "SERVER-API");

            var settings = _settingsService.GetUserSettings();
            ChangeClientType(settings.MediaServer?.Type);
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
            _logger.Debug($"Testing new API key on {url}");
            _logger.Debug($"API key used: {apiKey}");
            ChangeClientType(type);
            var oldApiKey = _baseHttpClient.ApiKey;
            var oldUrl = _baseHttpClient.BaseUrl;
            _baseHttpClient.ApiKey = apiKey;
            _baseHttpClient.BaseUrl = url;

            var info = await _baseHttpClient.GetServerInfo();
            if (info != null)
            {
                _logger.Debug("new API key works!");
                return true;
            }
            
            _baseHttpClient.ApiKey = oldApiKey;
            _baseHttpClient.BaseUrl = oldUrl;

            _logger.Debug("new API key not working!");
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
            _logger.Debug($"Pinging server on {url}");
            var oldUrl = _baseHttpClient.BaseUrl;
            _baseHttpClient.BaseUrl = url;
            
            var result = await _baseHttpClient.Ping();

            _baseHttpClient.BaseUrl = oldUrl;
            return result;
        }
        
        public async Task<bool> PingMediaServer()
        {
            _logger.Debug($"Pinging server on {_settingsService.GetUserSettings().MediaServer.Address}");
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

        public Task<List<MediaServerUser>> GetAllUsers()
        {
            return _mediaServerRepository.GetAllUsers();
        }

        public async Task<List<MediaServerUser>> GetAllAdministrators()
        {
            var administrators = await _mediaServerRepository.GetAllAdministrators();

            if (administrators.Any())
            {
                return administrators;
            }

            await GetAndProcessUsers();
            return await _mediaServerRepository.GetAllAdministrators();
        }

        public Task<MediaServerUser> GetUserById(string id)
        {
            return _mediaServerRepository.GetUserById(id);
        }

        public Card<int> GetViewedEpisodeCountByUserId(string id)
        {
            var episodeIds = _sessionService.GetMediaIdsForUser(id, PlayType.Episode);
            return new Card<int>
            {
                Title = Constants.Users.TotalWatchedEpisodes,
                Value = episodeIds.Count()
            };
        }

        public Card<int> GetViewedMovieCountByUserId(string id)
        {
            var movieIds = _sessionService.GetMediaIdsForUser(id, PlayType.Movie);
            return new Card<int>
            {
                Title = Constants.Users.TotalWatchedMovies,
                Value = movieIds.Count()
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

        public int GetUserViewCount(string id)
        {
            return _sessionService.GetPlayCountForUser(id);
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
                library.Sync = currentLibraries
                    .FirstOrDefault(x => x.Id == library.Id)?
                    .Sync ?? false;
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

        private UserMediaView CreateUserMediaViewFromEpisode(Play play, Device device)
        {
            //var episode = _showRepository.GetEpisodeById(play.play.MediaId);
            //TODO: fix implementation
            Episode episode = null;
            if (episode == null)
            {
                throw new BusinessException("EPISODENOTFOUND");
            }

            //var season = _showRepository.GetSeasonById(play.ParentId);
            //var seasonNumber = season.IndexNumber;
            //var name = $"{episode.ShowName} - {seasonNumber}x{episode.IndexNumber} - {episode.Name}";

            //var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            //var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            //var watchedTime = endedPlaying - startedPlaying;


            return new UserMediaView
            {
                Id = episode.Id,
                //Name = name,
                //ParentId = episode.ParentId,
                //Primary = episode.Primary,
                //StartedWatching = startedPlaying,
                //EndedWatching = endedPlaying,
                //WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, episode),
                DeviceId = device.Id,
                DeviceLogo = device?.IconUrl ?? ""
            };
        }

        private decimal? CalculateWatchedPercentage(Play play, long? runTimeTicks)
        {
            decimal? watchedPercentage = null;
            if (runTimeTicks.HasValue)
            {
                var playStates = play.PlayStates.Where(x => x.PositionTicks.HasValue).ToList();
                var watchedTicks = playStates.Max(x => x.PositionTicks) -
                                   playStates.Min(x => x.PositionTicks);
                watchedPercentage = Math.Round((watchedTicks.Value / (decimal) runTimeTicks) * 1000) / 10;
            }

            return watchedPercentage;
        }

        private decimal? CalculateWatchedPercentage(Play play, Extra media)
        {
            return CalculateWatchedPercentage(play, media.RunTimeTicks);
        }

        private void ChangeClientType(ServerType? type)
        {
            var realType = type ?? ServerType.Emby;
            _baseHttpClient = _clientStrategy.CreateHttpClient(realType);
            var settings = _settingsService.GetUserSettings();
            var appSettings = _settingsService.GetAppSettings();

            settings.MediaServer ??= new MediaServerSettings();

            _baseHttpClient.SetDeviceInfo(
                settings.AppName,
                settings.MediaServer.AuthorizationScheme,
                appSettings.Version.ToCleanVersionString(),
                settings.Id.ToString(),
                settings.MediaServer.UserId);

            settings.MediaServer.Type = realType;
            _settingsService.SaveUserSettingsAsync(settings);
        }
    }
}