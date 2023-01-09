using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace EmbyStat.Clients.Base.WebSocket;

public class WebSocketApi : IWebSocketApi, IDisposable
{
    public event EventHandler OnWebSocketClosed;
    public event EventHandler<GenericEventArgs<JArray>> UserDataChanged;
    public event EventHandler<GenericEventArgs<string>> UserDeleted;
    public event EventHandler<GenericEventArgs<JObject>> UserUpdated;
    public event EventHandler<EventArgs> ServerRestarting;
    public event EventHandler<EventArgs> ServerShuttingDown;
    public event EventHandler OnWebSocketConnected;
    public event EventHandler<GenericEventArgs<JArray>> SessionsUpdated;
    public event EventHandler<EventArgs> RestartRequired;

    private readonly IWebSocketHandler _handlerWebSocket;
    private readonly ILogger<WebSocketApi> _logger;

    private string ApiUrl { get; set; }
    public string AccessToken { get; set; }
    public string DeviceId { get; set; }

    public WebSocketApi(IWebSocketHandler handlerWebSocket, ILogger<WebSocketApi> logger)
    {
        _handlerWebSocket = handlerWebSocket;
        _logger = logger;
    }

    public async Task OpenWebSocket(string url, string accessToken, string deviceId)
    {
        if (!IsWebSocketOpenOrConnecting)
        {
            ApiUrl = url;
            AccessToken = accessToken;
            DeviceId = deviceId;

            await CloseWebSocket();
            await EnsureConnectionAsync();
        }
    }

    public async Task CloseWebSocket()
    {
        //await _clientWebSocket.CloseConnection();
    }

    private Task EnsureConnectionAsync()
    {
        if (IsWebSocketOpenOrConnecting)
        {
            return Task.CompletedTask;
        }

        var url = GetWebSocketUrl(ApiUrl);
        try
        {
            _logger.LogInformation($"Connecting to {url}");

            //_clientWebSocket.OnReceiveBytes = OnMessageReceived;
            //_clientWebSocket.OnReceive = OnMessageReceived;
            //_clientWebSocket.Closed += ClientWebSocketClosed;
            //_clientWebSocket.Connected += ClientWebSocketConnected;

            //await _clientWebSocket.ConnectAsync(url);                
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error connecting to {url}");
        }

        return Task.CompletedTask;
    }

    private void ClientWebSocketConnected(object sender, EventArgs e)
    {
        OnWebSocketConnected?.Invoke(this, EventArgs.Empty);
        _logger.LogInformation("Web socket connection opened.");
    }

    private void ClientWebSocketClosed(object sender, EventArgs e)
    {
        OnWebSocketClosed?.Invoke(this, EventArgs.Empty);
    }

    private Task SendWebSocketMessage<T>(string messageName, T data)
    {
        return SendWebSocketMessage(messageName, data, CancellationToken.None);
    }

    private async Task SendWebSocketMessage<T>(string messageName, T data, CancellationToken cancellationToken)
    {
        var bytes = GetMessageBytes(messageName, data);
        try
        {
            //await _clientWebSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending web socket message");
            throw;
        }
    }

    public Task StartReceivingSessionUpdates(int intervalMs)
    {
        return SendWebSocketMessage("SessionsStart", string.Format("{0},{0}", intervalMs));
    }

    public Task StopReceivingSessionUpdates()
    {
        return SendWebSocketMessage("SessionsStop", string.Empty);
    }

    private string GetWebSocketUrl(string serverAddress)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            throw new ArgumentException("Cannot open web socket without an access token.");
        }

        return serverAddress + "/embywebsocket?api_key=" + AccessToken + "&deviceId=" + DeviceId;
    }

    private void OnMessageReceived(byte[] bytes)
    {
        var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        OnMessageReceived(json);
    }

    private void OnMessageReceived(string json)
    {
        try
        {
            OnMessageReceivedInternal(json);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in OnMessageReceivedInternal");
        }
    }

    private void OnMessageReceivedInternal(string json)
    {
        var messageType = GetMessageType(json);
        switch (messageType)
        {
            case "RestartRequired":
                FireEvent(RestartRequired, this, EventArgs.Empty);
                break;
            case "ServerRestarting":
                FireEvent(RestartRequired, this, EventArgs.Empty);
                break;
            case "ServerShuttingDown":
                FireEvent(ServerShuttingDown, this, EventArgs.Empty);
                break;
            case "UserDeleted":
                var userId = JsonConvert.DeserializeObject<WebSocketMessage<string>>(json)?.Data;
                FireEvent(UserDeleted, this, new GenericEventArgs<string> {Argument = userId});
                break;
            case "UserUpdated":
                var user = JsonConvert.DeserializeObject<WebSocketMessage<JObject>>(json)?.Data;
                FireEvent(UserUpdated, this, new GenericEventArgs<JObject> {Argument = user});
                break;
            case "Sessions":
                var session = JsonConvert.DeserializeObject<WebSocketMessage<JArray>>(json)?.Data;
                FireEvent(SessionsUpdated, this, new GenericEventArgs<JArray> {Argument = session});
                break;
        }
    }

    public bool IsWebSocketOpenOrConnecting => false;

    private string GetMessageType(string json)
    {
        var message = JsonConvert.DeserializeObject<WebSocketMessage<object>>(json);
        return message.MessageType;
    }

    private void FireEvent<T>(EventHandler<T> handler, object sender, T args) where T : EventArgs
    {
        if (handler != null)
        {
            try
            {
                handler(sender, args);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in event handler");
            }
        }
    }

    private IEnumerable<byte> GetMessageBytes<T>(string messageName, T data)
    {
        var msg = new WebSocketMessage<T> {MessageType = messageName, Data = data};

        return SerializeToBytes(msg);
    }

    private static IEnumerable<byte> SerializeToBytes(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("obj");
        }

        using (var stream = new MemoryStream())
        {
            //JsonSerializerExtentions.SerializeToStream(obj, stream);
            return stream.ToArray();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            //_clientWebSocket?.Dispose();
        }
    }
}