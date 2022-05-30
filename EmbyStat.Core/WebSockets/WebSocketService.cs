using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Converters;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Sessions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Core.WebSockets;

public class WebSocketService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WebSocketService> _logger;

    private Timer _timer;

    public WebSocketService(IServiceScopeFactory scopeFactory, ILogger<WebSocketService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(TryToConnect, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private async void TryToConnect(object state)
    {
        using var scope = _scopeFactory.CreateScope();
        var settingsService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();

        if (!webSocketApi.IsWebSocketOpenOrConnecting)
        {
            var settings = settingsService.Get();
            if (!string.IsNullOrWhiteSpace(settings.UserConfig.MediaServer.ApiKey))
            {
                try
                {
                    var deviceId = settings.SystemConfig.Id.ToString();

                    webSocketApi.OnWebSocketConnected += ClientOnWebSocketConnected;
                    webSocketApi.OnWebSocketClosed += WebSocketApiOnWebSocketClosed;
                    webSocketApi.SessionsUpdated += WebSocketApiSessionsUpdated;
                    webSocketApi.UserDataChanged += WebSocketApiUserDataChanged;
                    await webSocketApi.OpenWebSocket(settings.UserConfig.MediaServer.FullSocketAddress, settings.UserConfig.MediaServer.ApiKey, deviceId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to open socket connection to MediaServer");
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
        using (var scope = _scopeFactory.CreateScope())
        {
            try
            {
                var webSocketApi = scope.ServiceProvider.GetRequiredService<IWebSocketApi>();
                webSocketApi.SessionsUpdated -= WebSocketApiSessionsUpdated;
                webSocketApi.UserDataChanged -= WebSocketApiUserDataChanged;
                webSocketApi.OnWebSocketConnected -= ClientOnWebSocketConnected;
                webSocketApi.OnWebSocketClosed -= WebSocketApiOnWebSocketClosed;

                _timer.Change(5000, 5000);
            }
            catch (Exception)
            {
                _logger.LogInformation("Application is closing, socket is closed!");
            }
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
        _logger.LogInformation("EmbyUser data changed");
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