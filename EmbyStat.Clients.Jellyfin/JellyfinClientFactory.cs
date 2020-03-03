using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Jellyfin
{
    public class JellyfinClientFactory : IClientFactory
    {
        private readonly IHttpClient _httpClient;

        public JellyfinClientFactory(IJellyfinHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IHttpClient CreateHttpClient()
        {
            return _httpClient;
        }

        public IWebSocketClient CreateWebSocketClient()
        {
            throw new NotImplementedException();
        }

        public bool AppliesTo(ServerType type)
        {
            return type == ServerType.Jellyfin;
        }
    }
}
