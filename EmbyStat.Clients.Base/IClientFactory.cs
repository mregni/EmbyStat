using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base;

public interface IClientFactory
{
    IBaseHttpClient CreateHttpClient();
    IWebSocketHandler CreateWebSocketClient();
    bool AppliesTo(ServerType type);
}