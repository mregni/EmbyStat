using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Emby;

public class EmbyClientFactory : IClientFactory
{
    private readonly IEmbyBaseHttpClient _baseHttpClient;
    private readonly IEmbyWebSocketHandler _webSocketHandler;

    public EmbyClientFactory(IEmbyBaseHttpClient baseHttpClient, IEmbyWebSocketHandler webSocketHandler)
    {
        _baseHttpClient = baseHttpClient;
        _webSocketHandler = webSocketHandler;
    }

    public IBaseHttpClient CreateHttpClient()
    {
        return _baseHttpClient;
    }

    public IWebSocketHandler CreateWebSocketClient()
    {
        return _webSocketHandler;
    }

    public bool AppliesTo(ServerType type)
    {
        return type == ServerType.Emby;
    }
}