using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;
using MoreLinq;
using NLog;

namespace EmbyStat.Services
{
    public class MediaServerService : IMediaServerService
    {
        private IHttpClient _httpClient;
        private readonly IMediaServerRepository _mediaServerRepository;
        private readonly ISessionService _sessionService;
        private readonly ISettingsService _settingsService;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly IClientStrategy _clientStrategy;
        private readonly Logger _logger;

        public MediaServerService(IClientStrategy clientStrategy, IMediaServerRepository mediaServerRepository, ISessionService sessionService,
            ISettingsService settingsService, IMovieRepository movieRepository, IShowRepository showRepository)
        {
            _mediaServerRepository = mediaServerRepository;
            _sessionService = sessionService;
            _settingsService = settingsService;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _clientStrategy = clientStrategy;
            _logger = LogManager.GetCurrentClassLogger();

            var settings = _settingsService.GetUserSettings();
            ChangeClientType(settings.MediaServer?.ServerType);
        }

        #region Server
        public MediaServerUdpBroadcast SearchMediaServer(ServerType type)
        {
            ChangeClientType(type);
            var result = _httpClient.SearchServer();
            var settings = _settingsService.GetUserSettings();
            settings.MediaServer.ServerName = result.Name;
            _settingsService.SaveUserSettingsAsync(settings);

            if (!string.IsNullOrWhiteSpace(result.Address))
            {
                var serverType = type == ServerType.Emby ? "Emby" : "jellyfin";
                _logger.Info($"{Constants.LogPrefix.ServerApi}\t{ serverType } server found at: " + result.Address);
            }

            return result;
        }

        public ServerInfo GetServerInfo()
        {
            var server = _mediaServerRepository.GetServerInfo();
            if (server == null)
            {
                server = GetAndProcessServerInfo();
            }

            return server;
        }

        public bool TestNewApiKey(string url, string apiKey)
        {
            var oldApiKey = _httpClient.ApiKey;
            var oldUrl = _httpClient.BaseUrl;
            _httpClient.ApiKey = apiKey;
            _httpClient.BaseUrl = url;

            var info = _httpClient.GetServerInfo();
            if (info != null)
            {
                return true;
            }

            _httpClient.ApiKey = oldApiKey;
            _httpClient.BaseUrl = oldUrl;
            return false;
        }

        public EmbyStatus GetMediaServerStatus()
        {
            return _mediaServerRepository.GetEmbyStatus();
        }

        public bool PingMediaServer(string url)
        {
            _httpClient.BaseUrl = url;
            return _httpClient.Ping();
        }

        public void ResetMissedPings()
        {
            _mediaServerRepository.ResetMissedPings();
        }

        public void IncreaseMissedPings()
        {
            _mediaServerRepository.IncreaseMissedPings();
        }

        #endregion

        #region Plugin

        public List<PluginInfo> GetAllPlugins()
        {
            return _mediaServerRepository.GetAllPlugins();
        }

        #endregion

        #region Users

        public IEnumerable<EmbyUser> GetAllUsers()
        {
            return _mediaServerRepository.GetAllUsers();
        }

        public IEnumerable<EmbyUser> GetAllAdministrators()
        {
            var administrators = _mediaServerRepository.GetAllAdministrators().ToList();

            if (administrators.Any())
            {
                return administrators;
            }

            GetAndProcessUsers();
            return _mediaServerRepository.GetAllAdministrators();
        }

        public EmbyUser GetUserById(string id)
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
            var skip = page * size;
            var sessions = _sessionService.GetSessionsForUser(id).ToList();
            var plays = sessions.SelectMany(x => x.Plays).Skip(skip).Take(size).ToList();
            var deviceIds = sessions.Where(x => plays.Any(y => x.Plays.Any(z => z.Id == y.Id))).Select(x => x.DeviceId);

            var devices = _mediaServerRepository.GetDeviceById(deviceIds).ToList();

            foreach (var play in plays)
            {
                var currentSession = sessions.Single(x => x.Plays.Any(y => y.Id == play.Id));
                var device = devices.SingleOrDefault(x => x.Id == currentSession.DeviceId);

                if (play.Type == PlayType.Movie)
                {
                    yield return CreateUserMediaViewFromMovie(play, device);
                }
                else if (play.Type == PlayType.Episode)
                {
                    yield return CreateUserMediaViewFromEpisode(play, device);
                }

                //if media is null this means a user has watched something that is not yet in our DB
                //TODO: try starting a sync here
            }
        }

        public int GetUserViewCount(string id)
        {
            return _sessionService.GetPlayCountForUser(id);
        }

        #endregion

        #region JobHelpers

        public ServerInfo GetAndProcessServerInfo()
        {
            var server = _httpClient.GetServerInfo();
            _mediaServerRepository.UpsertServerInfo(server);
            return server;
        }

        public void GetAndProcessPluginInfo()
        {
            var plugins = _httpClient.GetInstalledPlugins();
            _mediaServerRepository.RemoveAllAndInsertPluginRange(plugins);
        }

        public void GetAndProcessUsers()
        {
            var usersJson = _httpClient.GetUsers();
            var users = usersJson.ConvertToUserList();
            _mediaServerRepository.UpsertUsers(users);

            var localUsers = _mediaServerRepository.GetAllUsers();
            var removedUsers = localUsers.Where(u => users.All(u2 => u2.Id != u.Id));
            _mediaServerRepository.MarkUsersAsDeleted(removedUsers);
        }

        public void GetAndProcessDevices()
        {
            var devicesJson = _httpClient.GetDevices();
            var devices = devicesJson.ConvertToDeviceList();
            _mediaServerRepository.UpsertDevices(devices);

            var localDevices = _mediaServerRepository.GetAllDevices();
            var removedDevices = localDevices.Where(d => devices.All(d2 => d2.Id != d.Id));
            _mediaServerRepository.MarkDevicesAsDeleted(removedDevices);
        }

        #endregion

        private UserMediaView CreateUserMediaViewFromMovie(Play play, Device device)
        {
            var movie = _movieRepository.GetMovieById(play.MediaId);
            if (movie == null)
            {
                throw new BusinessException("MOVIENOTFOUND");
            }

            var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            var watchedTime = endedPlaying - startedPlaying;

            return new UserMediaView
            {
                Id = movie.Id,
                Name = movie.Name,
                ParentId = movie.ParentId,
                Primary = movie.Primary,
                StartedWatching = startedPlaying,
                EndedWatching = endedPlaying,
                WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, movie),
                DeviceId = device?.Id ?? string.Empty,
                DeviceLogo = device?.IconUrl ?? string.Empty
            };
        }

        private UserMediaView CreateUserMediaViewFromEpisode(Play play, Device device)
        {
            //var episode = _showRepository.GetEpisodeById(play.play.MediaId);
            //TODO: fix implementation
            Episode episode = null;
            if (episode == null)
            {
                throw new BusinessException("EPISODENOTFOUND");
            }

            var season = _showRepository.GetSeasonById(play.ParentId);
            var seasonNumber = season.IndexNumber;
            var name = $"{episode.ShowName} - {seasonNumber}x{episode.IndexNumber} - {episode.Name}";

            var startedPlaying = play.PlayStates.Min(x => x.TimeLogged);
            var endedPlaying = play.PlayStates.Max(x => x.TimeLogged);
            var watchedTime = endedPlaying - startedPlaying;


            return new UserMediaView
            {
                Id = episode.Id,
                Name = name,
                ParentId = episode.ParentId,
                Primary = episode.Primary,
                StartedWatching = startedPlaying,
                EndedWatching = endedPlaying,
                WatchedTime = Math.Round(watchedTime.TotalSeconds),
                WatchedPercentage = CalculateWatchedPercentage(play, episode),
                DeviceId = device.Id,
                DeviceLogo = device?.IconUrl ?? ""
            };
        }

        private decimal? CalculateWatchedPercentage(Play play, Extra media)
        {
            decimal? watchedPercentage = null;
            if (media.RunTimeTicks.HasValue)
            {
                var playStates = play.PlayStates.Where(x => x.PositionTicks.HasValue).ToList();
                var watchedTicks = playStates.Max(x => x.PositionTicks) -
                                   playStates.Min(x => x.PositionTicks);
                watchedPercentage = Math.Round((watchedTicks.Value / (decimal)media.RunTimeTicks.Value) * 1000) / 10;
            }

            return watchedPercentage;
        }

        private void ChangeClientType(ServerType? type)
        {
            var realType = type ?? ServerType.Emby;
            _httpClient = _clientStrategy.CreateHttpClient(realType);
            var settings = _settingsService.GetUserSettings();
            var appSettings = _settingsService.GetAppSettings();

            if (settings.MediaServer == null)
            {
                settings.MediaServer = new MediaServerSettings();
            }

            _httpClient.SetDeviceInfo(settings.AppName, settings.MediaServer.AuthorizationScheme, appSettings.Version.ToCleanVersionString(), settings.Id.ToString());
            settings.MediaServer.ServerType = realType;
            _settingsService.SaveUserSettingsAsync(settings);
        }
    }
}
