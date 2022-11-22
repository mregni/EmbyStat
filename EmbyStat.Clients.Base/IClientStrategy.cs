using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base;

public interface IClientStrategy
{
    IBaseHttpClient CreateHttpClient(ServerType type);
    IWebSocketHandler CreateWebSocketClient(ServerType type);
    IMetadataClient CreateMetadataClient(MetadataServerType type);
}