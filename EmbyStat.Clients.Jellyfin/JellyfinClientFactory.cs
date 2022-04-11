using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Jellyfin;

public class JellyfinClientFactory : IClientFactory
{
    private readonly IJellyfinBaseHttpClient _baseHttpClient;

    public JellyfinClientFactory(IJellyfinBaseHttpClient baseHttpClient)
    {
        _baseHttpClient = baseHttpClient;
    }

    public IBaseHttpClient CreateHttpClient()
    {
        return _baseHttpClient;
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