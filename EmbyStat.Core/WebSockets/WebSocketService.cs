using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Sessions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Core.WebSockets;

public class WebSocketService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WebSocketService> _logger;

    public WebSocketService(IServiceScopeFactory scopeFactory, ILogger<WebSocketService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var settingsService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        var settings = settingsService.Get();
        
        var client = CreateClient(scope);
        if (string.IsNullOrWhiteSpace(settings.UserConfig.MediaServer.ApiKey))
        {
            return;
        }
        
        try
        {
            var deviceId = settings.SystemConfig.Id.ToString();
            var socketAddress = settings.UserConfig.MediaServer.FullSocketAddress;
            var apiKey = settings.UserConfig.MediaServer.ApiKey;

            client.OnWebSocketClosed += WebSocketApiOnWebSocketClosed;
            await client.OpenWebSocket(socketAddress, apiKey, deviceId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to open socket connection to MediaServer");
        }
    }

    private void WebSocketApiOnWebSocketClosed(object? sender, EventArgs e)
    {
        using var scope = _scopeFactory.CreateScope();
        var client = CreateClient(scope);
        
        try
        {
            client.OnWebSocketClosed -= WebSocketApiOnWebSocketClosed;
        }
        catch (Exception)
        {
            _logger.LogInformation("Application is closing, socket is closed!");
        }
    }

    private void WebSocketApiUserDataChanged(object? sender, GenericEventArgs<JArray> e)
    {
        _logger.LogInformation("EmbyUser data changed");
    }

    private void WebSocketApiSessionsUpdated(object? sender, GenericEventArgs<JArray> e)
    {
        using var scope = _scopeFactory.CreateScope();
        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
        var sessions = SessionConverter.ConvertToSessions(e.Argument).ToList();
        sessionService.ProcessSessions(sessions);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var client = CreateClient(scope);
        await client.CloseWebSocket();
    }
    
    private static IWebSocketHandler CreateClient(IServiceScope scope)
    {
        var settingsService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
        var strategy = scope.ServiceProvider.GetRequiredService<IClientStrategy>();
        
        var settings = settingsService.Get();
        return strategy.CreateWebSocketClient(settings.UserConfig.MediaServer.Type);
    }
}