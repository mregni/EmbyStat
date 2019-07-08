using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Common.Converters;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NLog;

namespace EmbyStat.Services
{
    public class WebSocketService : IWebSocketService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Logger _logger;

        private Timer _timer;

        public WebSocketService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TryToConnect, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async void TryToConnect(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
                var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();

                if (!webSocketApi.IsWebSocketOpenOrConnecting)
                {
                    var settings = settingsService.GetUserSettings();
                    if (!string.IsNullOrWhiteSpace(settings.Emby.AccessToken))
                    {
                        try
                        {
                            var deviceId = settingsService.GetUserSettings().Id.ToString();

                            webSocketApi.OnWebSocketConnected += ClientOnWebSocketConnected;
                            webSocketApi.OnWebSocketClosed += WebSocketApiOnWebSocketClosed;
                            webSocketApi.SessionsUpdated += WebSocketApiSessionsUpdated;
                            webSocketApi.UserDataChanged += WebSocketApiUserDataChanged;
                            await webSocketApi.OpenWebSocket(settings.FullEmbyServerAddress, settings.Emby.AccessToken, deviceId);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "Failed to open socket connection to Emby");
                            throw;
                        }
                    }
                }
                else
                {
                    _timer.Change(60000, 60000);
                }
            }
        }

        private void WebSocketApiOnWebSocketClosed(object sender, EventArgs e)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();
                webSocketApi.SessionsUpdated -= WebSocketApiSessionsUpdated;
                webSocketApi.UserDataChanged -= WebSocketApiUserDataChanged;
                webSocketApi.OnWebSocketConnected -= ClientOnWebSocketConnected;
                webSocketApi.OnWebSocketClosed -= WebSocketApiOnWebSocketClosed;

                _timer.Change(5000, 5000);
            }
        }

        private async void ClientOnWebSocketConnected(object sender, EventArgs e)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();
                await webSocketApi.StopReceivingSessionUpdates();
                await webSocketApi.StartReceivingSessionUpdates(10000);
            }
        }

        private void WebSocketApiUserDataChanged(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            _logger.Info("EmbyUser data changed");
        }

        private void WebSocketApiSessionsUpdated(object sender, Common.Models.GenericEventArgs<JArray> e)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                var sessions = SessionConverter.ConvertToSessions(e.Argument).ToList();
                sessionService.ProcessSessions(sessions);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();
                await webSocketApi.CloseWebSocket();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
