using System;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base
{
    public interface IClientFactory
    {
        IHttpClient CreateHttpClient();
        IWebSocketClient CreateWebSocketClient();
        bool AppliesTo(ServerType type);
    }
}
