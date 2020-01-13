using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;
using IEmbyHttpClient = EmbyStat.Clients.Emby.Http.IEmbyHttpClient;

namespace EmbyStat.Clients.Emby
{
    public class EmbyClientFactory : IClientFactory
    {
        private readonly IEmbyHttpClient _httpClient;

        public EmbyClientFactory(IEmbyHttpClient httpClient)
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
            return type == ServerType.Emby;
        }
    }
}
