using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;
using Newtonsoft.Json;
using NLog;

namespace EmbyStat.Services
{
    public class MediaServerService : IMediaServerService
    {
        private readonly IHttpClient _httpClient;
        private readonly IEmbyRepository _embyRepository;
        private readonly ISessionService _sessionService;
        private readonly ISettingsService _settingsService;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;
        private readonly Logger _logger;

        public MediaServerService(IClientStrategy clientStrategy, IEmbyRepository embyRepository, ISessionService sessionService,
            ISettingsService settingsService, IMovieRepository movieRepository, IShowRepository showRepository)
        {
            _embyRepository = embyRepository;
            _sessionService = sessionService;
            _settingsService = settingsService;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _logger = LogManager.GetCurrentClassLogger();

            var settings = _settingsService.GetUserSettings();
            _httpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.ServerType ?? ServerType.Emby);
        }

        #region Server
        public EmbyUdpBroadcast SearchEmby()
        {
            using var client = new UdpClient();
            var requestData = Encoding.ASCII.GetBytes("who is EmbyServer?");
            var serverEp = new IPEndPoint(IPAddress.Any, 7359);

            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7359));

            var timeToWait = TimeSpan.FromSeconds(2);

            var asyncResult = client.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
            if (asyncResult.IsCompleted)
            {
                try
                {
                    var receivedData = client.EndReceive(asyncResult, ref serverEp);
                    var serverResponse = Encoding.ASCII.GetString(receivedData);
                    var udpBroadcastResult = JsonConvert.DeserializeObject<EmbyUdpBroadcast>(serverResponse);

                    var settings = _settingsService.GetUserSettings();
                    settings.MediaServer.ServerName = udpBroadcastResult.Name;
                    _settingsService.SaveUserSettingsAsync(settings);

                    if (!string.IsNullOrWhiteSpace(udpBroadcastResult.Address))
                    {
                        _logger.Info($"{Constants.LogPrefix.ServerApi}\tEmby server found at: " + udpBroadcastResult.Address);
                    }

                    return udpBroadcastResult;

                }
                catch (Exception)
                {
                    // No data received, swallow exception and return empty object
                }
            }

            return new EmbyUdpBroadcast();
        }

        public ServerInfo GetServerInfo()
        {
            var server = _embyRepository.GetServerInfo();
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

        public EmbyStatus GetEmbyStatus()
        {
            return _embyRepository.GetEmbyStatus();
        }

        public bool PingMediaServer(string url)
        {
            _httpClient.BaseUrl = url;
            return _httpClient.Ping();
        }

        public void ResetMissedPings()
        {
            _embyRepository.ResetMissedPings();
        }

        public void IncreaseMissedPings()
        {
            _embyRepository.IncreaseMissedPings();
        }

        #endregion

        #region Plugin

        public List<PluginInfo> GetAllPlugins()
        {
            return _embyRepository.GetAllPlugins();
        }

        #endregion

        #region Users

        public IEnumerable<EmbyUser> GetAllUsers()
        {
            return _embyRepository.GetAllUsers();
        }

        public EmbyUser GetUserById(string id)
        {
            return _embyRepository.GetUserById(id);
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

            var devices = _embyRepository.GetDeviceById(deviceIds).ToList();

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
            _embyRepository.UpsertServerInfo(server);
            return server;
        }

        public void GetAndProcessPluginInfo()
        {
            var plugins = _httpClient.GetInstalledPlugins();
            _embyRepository.RemoveAllAndInsertPluginRange(plugins);
        }

        public void GetAndProcessUsers()
        {
            var usersJson = _httpClient.GetUsers();
            var users = usersJson.ConvertToUserList();
            _embyRepository.UpsertUsers(users);

            var localUsers = _embyRepository.GetAllUsers();
            var removedUsers = localUsers.Where(u => users.All(u2 => u2.Id != u.Id));
            _embyRepository.MarkUsersAsDeleted(removedUsers);
        }

        public void GetAndProcessDevices()
        {
            var devicesJson = _httpClient.GetDevices();
            var devices = devicesJson.ConvertToDeviceList();
            _embyRepository.UpsertDevices(devices);

            var localDevices = _embyRepository.GetAllDevices();
            var removedDevices = localDevices.Where(d => devices.All(d2 => d2.Id != d.Id));
            _embyRepository.MarkDevicesAsDeleted(removedDevices);
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
    }
}
