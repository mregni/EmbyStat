using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace EmbyStat.Services
{
    public class WebSocketService : IWebSocketService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IEventService _eventService;
        private readonly IWebSocketApi _webSocketApi;

        private Timer _timer;

        public WebSocketService(IConfigurationService configurationService, IEventService eventService, IWebSocketApi webSocketApi)
        {
            _configurationService = configurationService;
            _eventService = eventService;
            _webSocketApi = webSocketApi;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TryToConnect, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void TryToConnect(object state)
        {
            if (!_webSocketApi.IsWebSocketOpenOrConnecting)
            {
                var settings = _configurationService.GetServerSettings();
                if (!string.IsNullOrWhiteSpace(settings.AccessToken))
                {
                    try
                    {
                        _webSocketApi.OpenWebSocket(settings.FullEmbyServerAddress, settings.AccessToken, Constants.Emby.DeviceId);
                        _webSocketApi.OnWebSocketConnected += _client_OnWebSocketConnected;
                        _webSocketApi.OnWebSocketClosed += _webSocketApi_OnWebSocketClosed;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Failed to open socket connection to Emby");
                        throw;
                    }
                }
            }
            else
            {
                _timer.Change(60000, 60000);
            }
        }

        private void _webSocketApi_OnWebSocketClosed(object sender, EventArgs e)
        {
            _webSocketApi.SessionsUpdated -= _webSocketApi_SessionsUpdated;
            _webSocketApi.UserDataChanged -= _webSocketApi_UserDataChanged;

            _timer.Change(5000, 5000);
        }

        private async void _client_OnWebSocketConnected(object sender, EventArgs e)
        {
            await _webSocketApi.StopReceivingSessionUpdates();
            await _webSocketApi.StartReceivingSessionUpdates(10000);

            _webSocketApi.SessionsUpdated += _webSocketApi_SessionsUpdated;
            _webSocketApi.UserDataChanged += _webSocketApi_UserDataChanged;
        }

        private void _webSocketApi_UserDataChanged(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            Log.Information("User data changed");
        }

        private async void _webSocketApi_SessionsUpdated(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            var sessions = SessionConverter.ConvertToSessions(e.Argument).ToList();
            await _eventService.ProcessSessions(sessions);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _webSocketApi.CloseWebSocket();
        }
    }
}
