using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.Http.Model;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Net;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace EmbyStat.Clients.Emby.Http
{
    public class EmbyHttpClient : BaseHttpClient, IEmbyHttpClient
    {
        public EmbyHttpClient(IRestClient client) : base(client)
        {
            
        }

        public List<PluginInfo> GetInstalledPlugins()
        {
            var request = new RestRequest("Plugins", Method.GET);
            return ExecuteAuthenticatedCall<List<PluginInfo>>(request);
        }

        public ServerInfo GetServerInfo()
        {
            var request = new RestRequest("System/Info", Method.GET);
            return ExecuteAuthenticatedCall<ServerInfo>(request);
        }

        public List<FileSystemEntryInfo> GetLocalDrives()
        {
            var request = new RestRequest("Environment/Drives", Method.GET);
            return ExecuteAuthenticatedCall<List<FileSystemEntryInfo>>(request);
        }

        public JArray GetUsers()
        {
            var request = new RestRequest("Users", Method.GET);
            return ExecuteAuthenticatedCall<JArray>(request);
        }

        public JObject GetDevices()
        {
            var request = new RestRequest("Devices", Method.GET);
            return ExecuteAuthenticatedCall<JObject>(request);
        }

        public bool Ping()
        {
            var request = new RestRequest("System/Ping", Method.POST) { Timeout = 5000};

            try
            {
                return ExecuteCall(request) == "Emby server";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public QueryResult<BaseItemDto> GetItems(ItemQuery query)
        {
            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query);
            return ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
        }

        public BaseItemDto GetPersonByName(string personName)
        {
            var request = new RestRequest($"persons/{personName}", Method.GET);
            request.AddItemQueryAsParameters(new ItemQuery { Fields = new[] { ItemFields.PremiereDate } });
            return ExecuteAuthenticatedCall<BaseItemDto>(request);
        }

        public QueryResult<BaseItemDto> GetMediaFolders()
        {
            var request = new RestRequest("Library/MediaFolders", Method.GET);
            return ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
        }

        private T ExecuteCall<T>(IRestRequest request) where T : new()
        {
            request.AddHeader("X-MediaServer-Authorization", $"{AuthorizationScheme} {AuthorizationParameter}");

            var result = Client.Execute<T>(request);
            return result.Data;
        }

        private string ExecuteCall(IRestRequest request)
        {
            request.AddHeader("X-MediaServer-Authorization", $"{AuthorizationScheme} {AuthorizationParameter}");

            var result = Client.Execute(request);
            return result.Content;
        }

        private T ExecuteAuthenticatedCall<T>(IRestRequest request) where T : new()
        {
            request.AddHeader("X-MediaBrowser-Token", ApiKey);
            return ExecuteCall<T>(request);
        }
    }
}
