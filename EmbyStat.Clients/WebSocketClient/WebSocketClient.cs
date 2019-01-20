using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace EmbyStat.Clients.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient
    {
        private ClientWebSocket webSocket;
        public async Task Connect(string url, CancellationToken cancellationToken)
        {
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(url), cancellationToken);
                await Task.WhenAll(OnMessage(webSocket, cancellationToken));
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

        public async Task OnMessage(ClientWebSocket webSocket, CancellationToken cancellationToken)
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
                        var message = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(buffer));
                        var data = message["Data"]; //TODO map event on MessageType (switch?)
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                    Log.Error(ex, "ERROR ON MESSAGE");
                }
            }
        }
    }
}
