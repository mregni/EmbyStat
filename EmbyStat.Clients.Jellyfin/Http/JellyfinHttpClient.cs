using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.Http.Model;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EmbyStat.Clients.Jellyfin.Http
{
    public class JellyfinHttpClient : BaseHttpClient, IJellyfinHttpClient
    {
        public JellyfinHttpClient(IRestClient client) : base(client)
        {

        }

        public List<PluginInfo> GetInstalledPlugins()
        {
            throw new System.NotImplementedException();
        }

        public ServerInfo GetServerInfo()
        {
            throw new System.NotImplementedException();
        }

        public List<FileSystemEntryInfo> GetLocalDrives()
        {
            throw new System.NotImplementedException();
        }

        public JArray GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public JObject GetDevices()
        {
            throw new System.NotImplementedException();
        }

        public bool Ping()
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BaseItemDto> GetItems(ItemQuery query)
        {
            throw new System.NotImplementedException();
        }

        public BaseItemDto GetPersonByName(string personName)
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<BaseItemDto> GetMediaFolders()
        {
            throw new System.NotImplementedException();
        }
    }
}
