using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Sessions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Websocket.Client;

namespace EmbyStat.Clients.Base.WebSocket;

public abstract class WebSocketHandler : IWebSocketHandler
{
    public event EventHandler OnWebSocketClosed;
    public event EventHandler<GenericEventArgs<WebSocketSession[]>> SessionsUpdated;

    private readonly ILogger<WebSocketHandler> _logger;
    private WebsocketClient _client;

    protected WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    public Task OpenWebSocket(string url, string accessToken, string deviceId)
    {
        var socketUrl = GetWebSocketUrl(url, accessToken, deviceId);
        _logger.LogDebug("Opening websocket on {SocketUrl}", socketUrl);

        _client = new WebsocketClient(new Uri(socketUrl));
        _client.ReconnectTimeout = TimeSpan.FromSeconds(60);
        _client.ReconnectionHappened.Subscribe(info =>
        {
            _logger.LogInformation("Reconnection happened, type: {Type}", info.Type);
            StartReceivingSessionUpdates(1500);
        });

        _client.MessageReceived.Subscribe(msg => OnMessageReceivedInternal(msg.Text));

        return _client.Start();
    }

    public Task CloseWebSocket()
    {
        return _client.Stop(WebSocketCloseStatus.NormalClosure, "Closure is triggered by application");
    }

    private void OnMessageReceivedInternal(string msg)
    {
        _logger.LogInformation("Message received: {Message}", msg);
        var messageType = GetMessageType(msg);
        switch (messageType)
        {
            case "Sessions":
                var sessions = JsonConvert.DeserializeObject<WebSocketMessage<WebSocketSession[]>>(msg)?.Data;
                FireEvent(SessionsUpdated, this, new GenericEventArgs<WebSocketSession[]> {Argument = sessions});
                break;
        }
    }

    private void FireEvent<T>(EventHandler<T> handler, object sender, T args) where T : EventArgs
    {
        if (handler == null)
        {
            return;
        }
        
        try
        {
            handler(sender, args);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in event handler");
        }
    }

    private string GetMessageType(string json)
    {
        var message = JsonConvert.DeserializeObject<WebSocketMessage<object>>(json);
        return message?.MessageType;
    }

    public void StartReceivingSessionUpdates(int intervalMs)
    {
        SendWebSocketMessage("SessionsStart", $"{intervalMs},{intervalMs}");
    }

    public void StopReceivingSessionUpdates()
    {
        SendWebSocketMessage("SessionsStop", string.Empty);
    }

    private void SendWebSocketMessage<T>(string messageName, T data)
    {
        var message = new WebSocketMessage<T> {MessageType = messageName, Data = data};
        _logger.LogDebug("Sending web socket message: {Message}", message);
        try
        {
            _client.Send(message.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending web socket message");
            throw;
        }
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