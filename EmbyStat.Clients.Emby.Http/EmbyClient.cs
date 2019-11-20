using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Net;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace EmbyStat.Clients.Emby.Http
{
    public class EmbyClient : IEmbyClient
    {
        private string DeviceName { get; set; }
        private string ApplicationVersion { get; set; }
        private string DeviceId { get; set; }

        private string apiKey { get; set; }
        public string ApiKey
        {
            get => apiKey;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                apiKey = value;
            }
        }

        public string BaseUrl
        {
            get => _client.BaseUrl?.ToString() ?? string.Empty;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _client.BaseUrl = new Uri(value);
            }
        }

        private string AuthorizationScheme { get; set; }
        private string AuthorizationParameter => $"Client=\"other\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";

        private readonly IRestClient _client;

        public EmbyClient(IRestClient client)
        {
            _client = client.UseSerializer(new JsonNetSerializer());
        }

        public void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId)
        {
            AuthorizationScheme = authorizationScheme;
            ApplicationVersion = applicationVersion;
            DeviceId = deviceId;
            DeviceName = deviceName;
        }

        public Task<List<PluginInfo>> GetInstalledPluginsAsync()
        {
            var request = new RestRequest("Plugins", Method.GET);
            return ExecuteAuthenticatedCall<List<PluginInfo>>(request);
        }

        public Task<ServerInfo> GetServerInfoAsync()
        {
            var request = new RestRequest("System/Info", Method.GET);
            return ExecuteAuthenticatedCall<ServerInfo>(request);
        }

        public Task<List<FileSystemEntryInfo>> GetLocalDrivesAsync()
        {
            var request = new RestRequest("Environment/Drives", Method.GET);
            return ExecuteAuthenticatedCall<List<FileSystemEntryInfo>>(request);
        }

        public Task<JArray> GetEmbyUsersAsync()
        {
            var request = new RestRequest("Users", Method.GET);
            return ExecuteAuthenticatedCall<JArray>(request);
        }

        public Task<JObject> GetEmbyDevicesAsync()
        {
            var request = new RestRequest("Devices", Method.GET);
            return ExecuteAuthenticatedCall<JObject>(request);
        }

        public async Task<string> PingEmbyAsync()
        {
            var request = new RestRequest("System/Ping", Method.POST) { Timeout = 5000};

            try
            {
                return await ExecuteCall<string>(request);
            }
            catch (Exception)
            {
                return "Ping failed";
            }
        }

        public Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query)
        {
            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query);
            return ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
        }

        public Task<BaseItemDto> GetPersonByNameAsync(string personName)
        {
            var request = new RestRequest($"persons/{personName}", Method.GET);
            request.AddItemQueryAsParameters(new ItemQuery { Fields = new[] { ItemFields.PremiereDate } });
            return ExecuteAuthenticatedCall<BaseItemDto>(request);
        }

        public async Task<QueryResult<BaseItemDto>> GetMediaFoldersAsync()
        {
            var request = new RestRequest("Library/MediaFolders", Method.GET);
            var result = await ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return result;
        }

        private async Task<T> ExecuteCall<T>(IRestRequest request)
        {
            request.AddHeader("X-Emby-Authorization", $"{AuthorizationScheme} {AuthorizationParameter}");

            var result = await _client.ExecuteTaskAsync<T>(request);
            return result.Data;
        }

        private Task<T> ExecuteAuthenticatedCall<T>(IRestRequest request)
        {
            request.AddHeader("X-MediaBrowser-Token", ApiKey);
            return ExecuteCall<T>(request);
        }
    }
}
