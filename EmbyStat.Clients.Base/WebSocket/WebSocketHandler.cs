using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;
using Websocket.Client;

namespace EmbyStat.Clients.Base.WebSocket;

public abstract class WebSocketHandler : IWebSocketHandler
{
    public event EventHandler OnWebSocketClosed;

    private string ApiUrl { get; set; }
    private string AccessToken { get; set; }
    private string DeviceId { get; set; }

    private readonly ILogger<WebSocketHandler> _logger;
    private WebsocketClient _client;

    protected WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    public Task OpenWebSocket(string url, string accessToken, string deviceId)
    {
        ApiUrl = GetWebSocketUrl(url, accessToken, deviceId);

        _client = new WebsocketClient(new Uri(ApiUrl));
        _client.ReconnectTimeout = TimeSpan.FromSeconds(60);
        _client.ReconnectionHappened.Subscribe(info =>
        {
            _logger.LogInformation($"Reconnection happened, type: {info.Type}");
        });

        _client.MessageReceived
            .Subscribe(msg =>
                _logger.LogInformation($"Message received: {msg}"));
        _client.Start();

        return Task.CompletedTask;
    }

    public Task CloseWebSocket()
    {
        throw new NotImplementedException();
    }

    public Task StartReceivingSessionUpdates(int intervalMs)
    {
        throw new NotImplementedException();
    }

    public Task StopReceivingSessionUpdates()
    {
        throw new NotImplementedException();
    }

    private string GetWebSocketUrl(string serverAddress, string token, string deviceId)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Cannot open web socket without an access token.");
        }

        return serverAddress + "/embywebsocket?api_key=" + token + "&deviceId=" + deviceId;
    }
}