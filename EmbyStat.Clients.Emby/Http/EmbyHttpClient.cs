using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace EmbyStat.Clients.Emby.Http
{
    public class EmbyHttpClient : BaseHttpClient, IEmbyHttpClient
    {
        public EmbyHttpClient(IRestClient client, IHttpContextAccessor accessor) : base(client, accessor)
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
