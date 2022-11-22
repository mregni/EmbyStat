using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Clients.Jellyfin.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Jellyfin;

public class JellyfinClientFactory : IClientFactory
{
    private readonly IJellyfinBaseHttpClient _baseHttpClient;
    private readonly IJellyfinWebSocketHandler _webSocketHandler;

    public JellyfinClientFactory(IJellyfinBaseHttpClient baseHttpClient, IJellyfinWebSocketHandler webSocketHandler)
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
        return type == ServerType.Jellyfin;
    }
}