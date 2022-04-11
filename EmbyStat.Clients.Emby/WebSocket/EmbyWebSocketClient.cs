using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.WebSocket;
using NLog;
using WebSocket4Net;
using WebSocketState = WebSocket4Net.WebSocketState;

namespace EmbyStat.Clients.Emby.WebSocket;

public class EmbyWebSocketClient : IWebSocketClient, IDisposable
{
    private WebSocket4Net.WebSocket _socket;
    public WebSocketState State => _socket?.State ?? WebSocketState.Closed;
    public event EventHandler Closed;
    public event EventHandler Connected;
    public Action<byte[]> OnReceiveBytes { get; set; }
    public Action<string> OnReceive { get; set; }
        
    public Task ConnectAsync(string url)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        try
        {
            _socket = new WebSocket4Net.WebSocket(url);

            _socket.MessageReceived += WebsocketMessageReceived;
            _socket.Opened += (sender, args) =>
            {
                Connected?.Invoke(this, EventArgs.Empty);
                taskCompletionSource.TrySetResult(true);
            };
            _socket.Closed += SocketClosed;

            _socket.Open();
        }
        catch (Exception ex)
        {
            _socket = null;

            taskCompletionSource.TrySetException(ex);
        }

        return taskCompletionSource.Task;
    }

    public async Task CloseConnection()
    {
        if (_socket?.State == WebSocketState.Open || _socket?.State == WebSocketState.Connecting)
        {
            await _socket?.CloseAsync();
        }
    }

    void SocketClosed(object sender, EventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    void WebsocketMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        OnReceive?.Invoke(e.Message);
    }

    public Task SendAsync(byte[] bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken)
    {
        return Task.Run(() => _socket.Send(bytes, 0, bytes.Length), cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _socket != null)
        {
            var state = State;

            if (state == WebSocketState.Open || state == WebSocketState.Connecting)
            {
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("Sending web socket close message");
                _socket.Close();
            }

            _socket = null;
        }
    }
}