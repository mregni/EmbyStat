using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Emby;

public class EmbyClientFactory : IClientFactory
{
    private readonly IEmbyBaseHttpClient _baseHttpClient;

    public EmbyClientFactory(IEmbyBaseHttpClient baseHttpClient)
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
        return type == ServerType.Emby;
    }
}