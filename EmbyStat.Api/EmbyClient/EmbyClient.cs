using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient.Cryptography;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Common;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Users;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EmbyStat.Api.EmbyClient
{
	public class EmbyClient : BaseClient<EmbyClient>, IEmbyClient
	{
        public EmbyClient(ICryptographyProvider cryptographyProvider, IJsonSerializer jsonSerializer, IAsyncHttpClient httpClient)
		: base(cryptographyProvider, jsonSerializer, httpClient)
		{
			
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
			var bytes = Encoding.UTF8.GetBytes(password);
			var args = new Dictionary<string, string>
			{
				["username"] = Uri.EscapeDataString(username),
				["pw"] = password,
				["password"] = BitConverter.ToString(CryptographyProvider.CreateSha1(bytes)).Replace("-", string.Empty),
				["passwordMD5"] = GetConnectPasswordMd5(password)
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

		public async Task<SystemInfo> GetServerInfoAsync()
		{
			var url = GetApiUrl("System/Info");

            Log.Information($"{Constants.LogPrefix.EmbyClient}\tAsking Emby for server info");
            using (var stream = await GetSerializedStreamAsync(url))
			{
				return DeserializeFromStream<SystemInfo>(stream);
			}
		}

		public async Task<List<Drive>> GetLocalDrivesAsync()
		{
			var url = GetApiUrl("Environment/Drives");

            Log.Information($"{Constants.LogPrefix.EmbyClient}\tAsking Emby for local drives");
			using (var stream = await GetSerializedStreamAsync(url))
            {
				return DeserializeFromStream<List<Drive>>(stream);
			}
		}

		public async Task<string> PingEmbyAsync()
		{
			var url = GetApiUrl("System/Ping");
			var args = new Dictionary<string, string>();

		    Log.Information($"{Constants.LogPrefix.EmbyClient}\tSending a ping to Emby");
            return await PostAsyncToString(url, args, CancellationToken.None);
		}

	    public async Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetItemListUrl($"Users/{query.UserId}/Items", query);

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
	        }
	    }

	    public async Task<BaseItemDto> GetItemAsync(ItemQuery personQuery, string personId, CancellationToken cancellationToken)
	    {
	        var url = GetItemListUrl($"Users/{personQuery.UserId}/Items/{personId}", personQuery);

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<BaseItemDto>(stream);
	        }
	    }

        public async Task<Folder> GetRootFolderAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
	    {
	        var url = GetApiUrl($"/Users/{userId}/Items/Root");

	        using (var stream = await GetSerializedStreamAsync(url, cancellationToken))
	        {
	            return DeserializeFromStream<Folder>(stream);
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
