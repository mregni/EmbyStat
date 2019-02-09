using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using Serilog;
using WebSocket4Net;
using WebSocketState = WebSocket4Net.WebSocketState;

namespace EmbyStat.Clients.Emby.WebSocket
{
    public class ClientWebSocket : IClientWebSocket
    {
        private WebSocket4Net.WebSocket _socket;
        public WebSocketState State => _socket?.State ?? WebSocketState.Closed;
        public event EventHandler Closed;
        public Action<byte[]> OnReceiveBytes { get; set; }
        public Action<string> OnReceive { get; set; }
        
        public Task ConnectAsync(string url)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            try
            {
                _socket = new WebSocket4Net.WebSocket(url);

                _socket.MessageReceived += websocket_MessageReceived;
                _socket.Open();

                _socket.Opened += (sender, args) => taskCompletionSource.TrySetResult(true);
                _socket.Closed += _socket_Closed;
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
                await _socket.CloseAsync();
            }
        }

        void _socket_Closed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnReceive?.Invoke(e.Message);
        }

        public Task SendAsync(byte[] bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => _socket.Send(bytes, 0, bytes.Length), cancellationToken);
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                var state = State;

                if (state == WebSocketState.Open || state == WebSocketState.Connecting)
                {
                    Log.Information("Sending web socket close message");
                    _socket.Close();
                }

                _socket = null;
            }
        }
    }
}
