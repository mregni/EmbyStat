using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EmbyStat.Common.Models;
using Newtonsoft.Json;

namespace EmbyStat.Clients.Base.Http
{
    public class ServerSearcher : IDisposable
    {
        private readonly UdpClient _client;
        public bool isDisposed { get; set; }
        private IPEndPoint _to;
        public event EventHandler<MediaServerUdpBroadcast> MediaServerFound;

        public ServerSearcher(IPEndPoint to)
        {
            isDisposed = false;
            _to = to;
            var from = new IPEndPoint(0, 0);
            _client = new UdpClient { EnableBroadcast = true, MulticastLoopback = true };
            _client.Client.Bind(from);
            _client.BeginReceive(OnMessageReceived, null);
        }

        public void Send(string message)
        {
            var requestData = Encoding.ASCII.GetBytes(message);
            try
            {
                _client.Send(requestData, requestData.Length, _to);
            }
            catch (Exception)
            {
                //swallowing failed calls
            }

        }

        private void OnMessageReceived(IAsyncResult ar)
        {
            if (!isDisposed)
            {
                var receiveBytes = _client.EndReceive(ar, ref _to);
                var receiveString = Encoding.ASCII.GetString(receiveBytes);

                var udpBroadcastResult = JsonConvert.DeserializeObject<MediaServerUdpBroadcast>(receiveString);

                MediaServerFound?.Invoke(this, udpBroadcastResult);
                _client.BeginReceive(OnMessageReceived, null);
            }
        }

        public void Dispose()
        {
            isDisposed = true;
            _client?.Dispose();
        }
    }
}
