﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Clients.Jellyfin.Http;

public class JellyfinBaseHttpClient : BaseHttpClient, IJellyfinBaseHttpClient
{
    public JellyfinBaseHttpClient(IHttpContextAccessor accessor, ILogger<JellyfinBaseHttpClient> logger,
        IRefitHttpClientFactory<IMediaServerApi> refitClient, IMapper mapper)
        : base(accessor, logger, refitClient, mapper)
    {
            
    }

    public Task<bool> Ping()
    {
        return Ping("\"Jellyfin Server\"");
    }

    public Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer()
    {
        return SearchServer("who is JellyfinServer?");
    }
}