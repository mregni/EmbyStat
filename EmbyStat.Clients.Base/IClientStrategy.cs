using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base
{
    public interface IClientStrategy
    {
        IHttpClient CreateHttpClient(ServerType type);
        IWebSocketClient CreateWebSocketClient(ServerType type);
    }
}
