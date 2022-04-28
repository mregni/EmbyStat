using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Clients.Emby.Http;

public class EmbyBaseHttpClient : BaseHttpClient, IEmbyBaseHttpClient
{
    public EmbyBaseHttpClient(IHttpContextAccessor accessor, ILogger<EmbyBaseHttpClient> logger,
        IRefitHttpClientFactory<IMediaServerApi> refitClient, IMapper mapper) 
        : base(accessor, logger, refitClient, mapper)
    {
            
    }

    public Task<bool> Ping()
    {
        return Ping("Emby Server");
    }

    public Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer()
    {
        return SearchServer("who is EmbyServer?");
    }
}