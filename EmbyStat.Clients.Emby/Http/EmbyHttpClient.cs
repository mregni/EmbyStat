using System.Collections.Generic;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using RestSharp;

namespace EmbyStat.Clients.Emby.Http
{
    public class EmbyHttpClient : BaseHttpClient, IEmbyHttpClient
    {
        public EmbyHttpClient(IRestClient client) : base(client)
        {
            
        }

        public MediaServerUdpBroadcast SearchServer()
        {
            return SearchServer("who is EmbyServer?");
        }

        public bool Ping()
        {
            return Ping("Emby server");
        }
    }
}
