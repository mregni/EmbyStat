using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Sockets.EmbyClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace EmbyStat.Clients.WebSocketClient
{
    public class EmbySocketClient : IEmbySocketClient
    {
        private ClientWebSocket webSocket;
        public async Task Connect(string url, CancellationToken cancellationToken)
        {
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(url), cancellationToken);
                await Task.WhenAll(ReceiveMessages(webSocket, cancellationToken));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ERROR ON CONNECTION");
            }
            finally
            {
                webSocket?.Dispose();
            }
        }

        public async Task Close(CancellationToken cancellationToken)
        {
            if (webSocket.State != WebSocketState.Closed)
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "EmbyStat is going offline", cancellationToken);
            }
        }

        public bool IsWebSocketConnectionOpen()
        {
            return webSocket.State == WebSocketState.Open;
        }

        private async Task ReceiveMessages(ClientWebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                    }
                    else
                    {
                        var rawMessage = Encoding.UTF8.GetString(buffer);
                        ProcessMessage(rawMessage);

                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                    Log.Error(ex, "ERROR ON MESSAGE");
                }
            }
        }

        private void ProcessMessage(string rawMessage)
        {
            var message = JsonConvert.DeserializeObject<EmbyMessage>(rawMessage);
            var boe = message.MessageType;
            switch (message.MessageType)
            {
                case "UserDataChanged":
                    break;
                default:
                    break;
            }
        }
    }
}
