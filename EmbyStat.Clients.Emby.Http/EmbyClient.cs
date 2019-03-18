using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Clients.EmbyClient.Net;
using EmbyStat.Common;
using EmbyStat.Common.Exceptions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Users;
using Newtonsoft.Json.Linq;
using Serilog;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace EmbyStat.Clients.Emby.Http
{
	public class EmbyClient : BaseClient, IEmbyClient
	{
        public EmbyClient(IAsyncHttpClient httpClient)
		: base( httpClient)
		{
			
		}

        public void SetDeviceInfo(string clientName, string authorizationScheme, string applicationVersion, string deviceId)
        {
            ClientName = clientName;
            AuthorizationScheme = authorizationScheme;
            ApplicationVersion = applicationVersion;

            Device = new Device
            {
                DeviceId = deviceId,
                DeviceName = clientName
            };
            ResetHttpHeaders();
        }

		public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username));
			}

			if (string.IsNullOrWhiteSpace(address))
			{
				throw new ArgumentNullException(nameof(address));
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException(nameof(password));
			}

			ServerAddress = address;
			var args = new Dictionary<string, string>
			{
				["username"] = Uri.EscapeDataString(username),
				["pw"] = password
			};

			var url = GetApiUrl("Users/AuthenticateByName");
            Log.Information($"{Constants.LogPrefix.EmbyClient}\tAuthenticating user {username} on Emby server on {ServerAddress}");
			var result = await PostAsync<AuthenticationResult>(url, args, CancellationToken.None);

			SetAuthenticationInfo(result.AccessToken, result.User.Id);

			return result;
		}

		public async Task<List<PluginInfo>> GetInstalledPluginsAsync()
		{
			var url = GetApiUrl("Plugins");
            
            Log.Information($"{Constants.LogPrefix.EmbyClient}\tAsking Emby for plugins");
			using (var stream = await GetSerializedStreamAsync(url))
			{
				return DeserializeFromStream<List<PluginInfo>>(stream);
			}
		}

		public async Task<ServerInfo> GetServerInfoAsync()
		{
			var url = GetApiUrl("System/Info");

            using (var stream = await GetSerializedStreamAsync(url))
			{
				return DeserializeFromStream<ServerInfo>(stream);
			}
		}

		public async Task<List<FileSystemEntryInfo>> GetLocalDrivesAsync()
		{
			var url = GetApiUrl("Environment/Drives");

			using (var stream = await GetSerializedStreamAsync(url))
            {
				return DeserializeFromStream<List<FileSystemEntryInfo>>(stream);
			}
		}

        public async Task<JArray> GetEmbyUsers()
        {
            var url = GetApiUrl("Users");

            using (var stream = await GetSerializedStreamAsync(url))
            {
                return DeserializeFromStream<JArray>(stream);
            }
        }

        public async Task<JObject> GetEmbyDevices()
        {
            var url = GetApiUrl("Devices");

            using (var stream = await GetSerializedStreamAsync(url))
            {
                return DeserializeFromStream<JObject>(stream);
            }
        }

        public async Task<string> PingEmbyAsync(CancellationToken cancellationToken)
		{
			var url = GetApiUrl("System/Ping");
			var args = new Dictionary<string, string>();

		    Log.Information($"{Constants.LogPrefix.EmbyClient}\tSending a ping to Emby");
            try
            {
                return await PostAsyncToString(url, args, 5000, cancellationToken);
            }
            catch (BusinessException e)
            {
                return await Task.Run(() => "Ping failed", cancellationToken);
            }
		}

	    public async Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetItemListUrl("Items", query);

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
	        }
	    }

        public async Task<BaseItemDto> GetPersonByNameAsync(string personName, CancellationToken cancellationToken)
	    {
	        var url = GetItemListUrl($"persons/{personName}", new ItemQuery());

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<BaseItemDto>(stream);
	        }
	    }

	    public async Task<QueryResult<BaseItemDto>> GetMediaFolders(CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetApiUrl("/Library/MediaFolders");

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
	        }
        }

	    public async Task<QueryResult<BaseItemDto>> GetPeopleAsync(PersonsQuery query, CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetItemByNameListUrl("Persons", query);

	        if (query.PersonTypes != null && query.PersonTypes.Length > 0)
	        {
	            url += "&PersonTypes=" + string.Join(",", query.PersonTypes);
	        }

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
	        }
	    }

	    public async Task<QueryResult<BaseItemDto>> GetGenresAsync(ItemsByNameQuery query, CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetItemByNameListUrl("Genres", query);

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
	        }
	    }

	    public void Dispose()
		{

		}
	}
}
