using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace EmbyStat.Clients.Emby.Http
{
    public class EmbyBaseHttpClient : BaseHttpClient, IEmbyBaseHttpClient
    {
        public EmbyBaseHttpClient(IRestClient client, IHttpContextAccessor accessor, 
            IRefitHttpClientFactory<INewBaseClient> refitClient, IMapper mapper) 
            : base(client, accessor, refitClient, mapper)
        {
            
        }

        public bool Ping()
        {
            return Ping("Emby Server");
        }

        public Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer()
        {
            return SearchServer("who is EmbyServer?");
        }
    }
}
