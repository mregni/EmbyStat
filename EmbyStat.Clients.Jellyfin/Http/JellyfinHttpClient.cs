using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Models;
using RestSharp;

namespace EmbyStat.Clients.Jellyfin.Http
{
    public class JellyfinHttpClient : BaseHttpClient, IJellyfinHttpClient
    {
        public JellyfinHttpClient(IRestClient client) : base(client)
        {
            
        }

        public bool Ping()
        {
            return Ping("\"Jellyfin Server\"");
        }

        public MediaServerUdpBroadcast SearchServer()
        {
            return SearchServer("who is JellyfinServer?");
        }
    }
}
