using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Common.Converters;
using EmbyStat.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Serilog;

namespace EmbyStat.Services
{
    public class WebSocketService : IWebSocketService, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly IEventService _eventService;
        private readonly IWebSocketApi _webSocketApi;

        private Timer _timer;

        public WebSocketService(ISettingsService settingsService, IEventService eventService, IWebSocketApi webSocketApi)
        {
            _settingsService = settingsService;
            _eventService = eventService;
            _webSocketApi = webSocketApi;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TryToConnect, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async void TryToConnect(object state)
        {
            if (!_webSocketApi.IsWebSocketOpenOrConnecting)
            {
                var settings = _settingsService.GetUserSettings();
                if (!string.IsNullOrWhiteSpace(settings.Emby.AccessToken))
                {
                    try
                    {
                        var deviceId = _settingsService.GetUserSettings().Id.ToString();

                        _webSocketApi.OnWebSocketConnected += ClientOnWebSocketConnected;
                        _webSocketApi.OnWebSocketClosed += WebSocketApiOnWebSocketClosed;
                        _webSocketApi.SessionsUpdated += WebSocketApiSessionsUpdated;
                        _webSocketApi.UserDataChanged += WebSocketApiUserDataChanged;
                        await _webSocketApi.OpenWebSocket(settings.FullEmbyServerAddress, settings.Emby.AccessToken, deviceId);
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

        private void WebSocketApiOnWebSocketClosed(object sender, EventArgs e)
        {
            _webSocketApi.SessionsUpdated -= WebSocketApiSessionsUpdated;
            _webSocketApi.UserDataChanged -= WebSocketApiUserDataChanged;
            _webSocketApi.OnWebSocketConnected -= ClientOnWebSocketConnected;
            _webSocketApi.OnWebSocketClosed -= WebSocketApiOnWebSocketClosed;

            _timer.Change(5000, 5000);
        }

        private async void ClientOnWebSocketConnected(object sender, EventArgs e)
        {
            await _webSocketApi.StopReceivingSessionUpdates();
            await _webSocketApi.StartReceivingSessionUpdates(10000);
        }

        private void WebSocketApiUserDataChanged(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            Log.Information("User data changed");
        }

        private async void WebSocketApiSessionsUpdated(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            var sessions = SessionConverter.ConvertToSessions(e.Argument).ToList();
            await _eventService.ProcessSessions(sessions);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _webSocketApi.CloseWebSocket();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
